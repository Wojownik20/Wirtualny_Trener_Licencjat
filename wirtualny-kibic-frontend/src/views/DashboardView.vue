<template>
  <div class="dashboard-page">
    <div class="dashboard-container" v-if="staff">
      <section class="dashboard-grid">
        <div class="left-column">
          <div class="welcome-section">
            <h1>Witaj {{ staff.firstName }}!</h1>
            <div class="welcome-line"></div>
          </div>

          <div class="section-block">
            <h2>Tabela Premier League :</h2>

            <div class="content-box standings-box">
              <p v-if="standingsLoading">Ładowanie tabeli...</p>

              <table v-else class="standings-table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Klub</th>
                    <th>M</th>
                    <th>W</th>
                    <th>R</th>
                    <th>P</th>
                    <th>+/-</th>
                    <th>PKT</th>
                  </tr>
                </thead>

                <tbody>
                  <tr
  v-for="team in visibleStandings"
  :key="team.rank"
  :class="[
    getTableRowClass(team.rank),
    { 'my-team-row': isMyTeam(team.teamName) }
  ]"
>
                    <td>{{ team.rank }}</td>
                    <td class="team-cell">
                      <img :src="team.teamLogo" :alt="team.teamName" />
                      {{ team.teamName }}
                    </td>
                    <td>{{ team.played }}</td>
                    <td>{{ team.wins }}</td>
                    <td>{{ team.draws }}</td>
                    <td>{{ team.losses }}</td>
                    <td>{{ team.goalsFor }}:{{ team.goalsAgainst }}</td>
                    <td class="points">{{ team.points }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="right-column">
          <div class="section-block">
            <h2>Ostatnie wyniki :</h2>

            <div class="content-box results-box">
              <p v-if="resultsLoading">Ładowanie wyników...</p>
              <p v-else-if="lastThreeResults.length === 0">Brak wyników.</p>

              <div v-else v-for="match in lastThreeResults" :key="match.fixtureId" class="result-row">
                <img :src="match.homeTeamLogo" :alt="match.homeTeamName" class="team-logo" />
                <span class="team-name home-team">{{ match.homeTeamName }}</span>
                <span class="score" :class="getResultClass(match)">
                  {{ match.homeGoals }} : {{ match.awayGoals }}
                </span>
                <span class="team-name away-team">{{ match.awayTeamName }}</span>
                <img :src="match.awayTeamLogo" :alt="match.awayTeamName" class="team-logo" />
              </div>
            </div>
          </div>

          <div class="section-block">
            <h2>Następne mecze :</h2>

            <div class="content-box results-box">
              <p v-if="resultsLoading">Ładowanie meczów...</p>
              <p v-else-if="nextThreeMatches.length === 0">Brak nadchodzących meczów.</p>

              <div v-else v-for="match in nextThreeMatches" :key="match.fixtureId" class="result-row">
                <img :src="match.homeTeamLogo" :alt="match.homeTeamName" class="team-logo" />
                <span class="team-name home-team">{{ match.homeTeamName }}</span>
                <span class="score match-date">{{ formatDate(match.date) }}</span>
                <span class="team-name away-team">{{ match.awayTeamName }}</span>
                <img :src="match.awayTeamLogo" :alt="match.awayTeamName" class="team-logo" />
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
import api from '../api/axios'

export default {
  data() {
    return {
      staff: null,
      seasonFixtures: [],
      resultsLoading: false,
      standings: [],
      standingsLoading: false
    }
  },

  computed: {
  lastThreeResults() {
    return this.seasonFixtures
      .filter(match => match.homeGoals !== null && match.awayGoals !== null)
      .sort((a, b) => new Date(b.date) - new Date(a.date))
      .slice(0, 3)
  },

  nextThreeMatches() {
    return this.seasonFixtures
      .filter(match => match.homeGoals === null && match.awayGoals === null)
      .sort((a, b) => new Date(a.date) - new Date(b.date))
      .slice(0, 3)
  },

  visibleStandings() {
    if (!this.staff || this.standings.length === 0) return []

    const normalize = value =>
      value?.toLowerCase()
        .replace(/&/g, 'and')
        .replace(/\bfc\b/g, '')
        .replace(/\bafc\b/g, '')
        .replace(/\bthe\b/g, '')
        .trim() || ''

    const teamName = normalize(this.staff.team)

    const teamIndex = this.standings.findIndex(team => {
      const standingTeamName = normalize(team.teamName)
      return standingTeamName.includes(teamName) || teamName.includes(standingTeamName)
    })

    if (teamIndex === -1) {
      return this.standings.slice(0, 8)
    }

    let start = teamIndex - 3
    let end = teamIndex + 5

    if (start < 0) {
      start = 0
      end = 8
    }

    if (end > this.standings.length) {
      end = this.standings.length
      start = Math.max(0, end - 8)
    }

    return this.standings.slice(start, end)
  }
},

  async mounted() {
    await this.fetchStaff()
    await this.fetchSeasonFixtures()
    await this.fetchStandings()
  },

  methods: {
    async fetchStaff() {
      try {
        const res = await api.get('/Staff/me')
        this.staff = res.data
      } catch (err) {
        console.error(err)
      }
    },

    getTableRowClass(rank) {
  if (rank === 1) return 'rank-first'
  if (rank >= 2 && rank <= 5) return 'rank-top'
  if (rank >= 18 && rank <= 20) return 'rank-relegation'
  return ''
},

    isMyTeam(teamName) {
  if (!this.staff) return false

  const normalize = value =>
    value?.toLowerCase()
      .replace(/&/g, 'and')
      .replace(/\bfc\b/g, '')
      .replace(/\bafc\b/g, '')
      .replace(/\bthe\b/g, '')
      .trim() || ''

  const staffTeam = normalize(this.staff.team)
  const tableTeam = normalize(teamName)

  return tableTeam.includes(staffTeam) || staffTeam.includes(tableTeam)
},

    async fetchSeasonFixtures() {
      try {
        this.resultsLoading = true

        const res = await api.get('/Fixtures/my-team-premier-league-season')
        this.seasonFixtures = res.data
      } catch (err) {
        console.error('Błąd pobierania wyników sezonu:', err)
      } finally {
        this.resultsLoading = false
      }
    },

    async fetchStandings() {
  try {
    this.standingsLoading = true

    const res = await api.get('/Fixtures/premier-league-standings')
    this.standings = res.data
  } catch (err) {
    console.error(err)
  } finally {
    this.standingsLoading = false
  }
},

    getResultClass(match) {
      if (!this.staff || match.homeGoals === null || match.awayGoals === null) return ''

      const normalize = value =>
        value
          ?.toLowerCase()
          .replace(/&/g, 'and')
          .replace(/\bfc\b/g, '')
          .replace(/\bafc\b/g, '')
          .replace(/\bthe\b/g, '')
          .trim() || ''

      const teamName = normalize(this.staff.team)
      const homeName = normalize(match.homeTeamName)
      const awayName = normalize(match.awayTeamName)

      const isHomeTeam = homeName.includes(teamName) || teamName.includes(homeName)
      const isAwayTeam = awayName.includes(teamName) || teamName.includes(awayName)

      if (isHomeTeam) {
        if (match.homeGoals > match.awayGoals) return 'win'
        if (match.homeGoals < match.awayGoals) return 'loss'
        return 'draw'
      }

      if (isAwayTeam) {
        if (match.awayGoals > match.homeGoals) return 'win'
        if (match.awayGoals < match.homeGoals) return 'loss'
        return 'draw'
      }

      return ''
    },

    formatDate(date) {
      return new Date(date).toLocaleString('pl-PL', {
        day: '2-digit',
        month: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
      })
    }
  }
}
</script>

<style scoped>
.dashboard-page {
  min-height: calc(100vh - 72px);
  background: linear-gradient(180deg, #5b0b739c 0%, #32003f 100%);
  padding: 36px 42px;
  font-family: 'PremierLeague', sans-serif;
}

.dashboard-container {
  max-width: 1400px;
  margin: 0 auto;
}

.dashboard-grid {
  display: grid;
  grid-template-columns: 1.15fr 0.85fr;
  gap: 56px;
  align-items: start;
}

.left-column,
.right-column {
  display: flex;
  flex-direction: column;
}

.welcome-section {
  margin-bottom: 28px;
}

.welcome-section h1 {
  margin: 0;
  color: white;
  font-size: 4.4rem;
  font-weight: 800;
  line-height: 1.05;
}

.welcome-line {
  margin-top: 10px;
  width: 380px;
  max-width: 100%;
  height: 3px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 999px;
}

.section-block {
  margin-bottom: 34px;
}

.section-block h2 {
  margin: 0 0 18px;
  color: white;
  font-size: 2.15rem;
  font-weight: 800;
}

.content-box {
  background: #3b0c7281;
  border-radius: 28px;
  color: white;
  box-shadow: 0 14px 30px rgba(0, 0, 0, 0.16);
}

.large-box {
  min-height: 310px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 34px;
}

.large-box p {
  margin: 0;
  font-size: 2rem;
  font-weight: 500;
  text-align: center;
}

.results-box {
  min-height: 240px;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 16px;
  padding: 28px;
}

.results-box p {
  margin: 0;
  font-size: 2rem;
  font-weight: 700;
  text-align: center;
}

.my-team-row {
  background: rgba(255, 255, 255, 0.66);
  border-radius: 10px;
}

.my-team-row td {
  font-weight: 900;
  background: rgba(114, 42, 209, 0.48);
}

.result-row {
  display: grid;
  grid-template-columns: 42px minmax(90px, 1fr) 90px minmax(90px, 1fr) 42px;
  align-items: center;
  gap: 12px;
  width: 100%;
  padding: 10px 12px;
  border-radius: 18px;
  background: rgba(255, 255, 255, 0.07);
}

.team-logo {
  width: 38px;
  height: 38px;
  object-fit: contain;
}

.team-name {
  font-size: 1.05rem;
  font-weight: 800;
  line-height: 1.1;
}

.home-team {
  text-align: left;
}

.away-team {
  text-align: right;
}

.score {
  text-align: center;
  font-size: 1.35rem;
  font-weight: 900;
  color: white;
  white-space: nowrap;
}

.match-date {
  font-size: 0.95rem;
}

.score.win {
  color: #4caf50;
}

.score.loss {
  color: #ff4d4d;
}

.score.draw {
  color: #ffffff;
}

.rank-first {
  background: rgba(21, 224, 28, 0.36);
}

.rank-top {
  background: rgba(26, 113, 184, 0.32);
}

.rank-relegation {
  background: rgba(243, 18, 18, 0.35);
}

.standings-box {
  padding: 20px;
  overflow-x: auto;
}

.standings-table {
  width: 100%;
  border-collapse: collapse;
  color: white;
  font-size: 0.9rem;
}

.standings-table th {
  text-align: left;
  opacity: 0.8;
  font-weight: 600;
  padding-bottom: 8px;
}

.standings-table td {
  padding: 8px 8px;
}

.team-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.standings-table tbody tr {
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.team-cell img {
  width: 35px;
  height: 35px;
  object-fit: contain;
}

.points {
  font-weight: 800;
}

@media (max-width: 1100px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
    gap: 22px;
  }

  .welcome-section h1 {
    font-size: 3.2rem;
  }

  .section-block h2 {
    font-size: 1.8rem;
  }

  .large-box p,
  .results-box p {
    font-size: 1.5rem;
  }

  .result-row {
    grid-template-columns: 34px minmax(70px, 1fr) 70px minmax(70px, 1fr) 34px;
  }

  .team-logo {
    width: 32px;
    height: 32px;
  }

  .team-name {
    font-size: 0.9rem;
  }

  .score {
    font-size: 1.1rem;
  }

  .match-date {
    font-size: 0.8rem;
  }
}

@media (max-width: 768px) {
  .dashboard-page {
    padding: 24px 20px;
  }

  .welcome-section h1 {
    font-size: 2.6rem;
  }

  .welcome-line {
    width: 220px;
  }

  .section-block h2 {
    font-size: 1.5rem;
  }

  .large-box {
    min-height: 220px;
  }

  .results-box {
    min-height: 180px;
  }
}
</style>