<template>
  <main class="fixtures-page">
  <section class="fixtures-layout">
    <div class="results-panel">
      <div class="section-header">
        <h1>Ostatnie wyniki :</h1>
        <div class="header-line"></div>
      </div>

      <div v-if="loading" class="info-box">Ładowanie wyników...</div>

      <div v-else-if="lastResults.length === 0" class="info-box">
        Brak rozegranych meczów.
      </div>

      <div v-else class="content-box results-box">
        <div class="results-scroll">
          <div
            v-for="match in lastResults"
            :key="match.fixtureId"
            class="result-card"
          >
            <h2>Kolejka {{ formatRound(match.round) }}</h2>

            <div class="match-box fixture-style">
              <div class="team-side home-side">
                <img :src="match.homeTeamLogo" alt="" />
                <span>{{ match.homeTeamName }}</span>
              </div>

              <div class="score-center" :class="getScoreClass(match)">
                {{ match.homeGoals }} : {{ match.awayGoals }}
              </div>

              <div class="team-side away-side">
                <span>{{ match.awayTeamName }}</span>
                <img :src="match.awayTeamLogo" alt="" />
              </div>
            </div>

            <div class="match-footer">
              <p class="date">{{ formatDate(match.date) }}</p>

              <button
                class="details-button"
                @click="openMatchDetails(match)"
              >
                Zobacz więcej
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="section-block standings-section">
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
  </section>

  <div
    v-if="selectedMatch"
    class="modal-overlay"
    @click.self="selectedMatch = null"
  >
    <div class="match-modal">
      <button class="modal-close" @click="selectedMatch = null">
        ×
      </button>

      <h2>Kolejka {{ formatRound(selectedMatch.round) }}</h2>

      <p class="modal-date">
        {{ formatDate(selectedMatch.date) }}
      </p>

      <div class="modal-score">
        <div class="modal-team">
          <img :src="selectedMatch.homeTeamLogo" alt="" />
          <span>{{ selectedMatch.homeTeamName }}</span>
        </div>

        <strong :class="getScoreClass(selectedMatch)">
          {{ selectedMatch.homeGoals }} :
          {{ selectedMatch.awayGoals }}
        </strong>

        <div class="modal-team right">
          <span>{{ selectedMatch.awayTeamName }}</span>
          <img :src="selectedMatch.awayTeamLogo" alt="" />
        </div>
      </div>

      <div class="stats-grid">
        <div class="stat-box">
          <span>Rozgrywki</span>
          <strong>
            {{ selectedMatch.leagueName || 'Premier League' }}
          </strong>
        </div>

        <div class="stat-box">
          <span>Status</span>
          <strong>
            {{ selectedMatch.status || 'Zakończony' }}
          </strong>
        </div>

        <div class="stat-box">
          <span>Stadion</span>
          <strong>
            {{ selectedMatch.venueName || 'Brak danych' }}
          </strong>
        </div>

        <div class="stat-box">
          <span>Data meczu</span>
          <strong>
            {{ formatDate(selectedMatch.date) }}
          </strong>
        </div>
      </div>

      <div v-if="statsLoading" class="info-box">
        Ładowanie statystyk...
      </div>

      <div v-else-if="selectedMatchStats" class="stats-list">
        <div
          v-for="stat in selectedMatchStats.statistics"
          :key="stat.type"
          class="stat-row"
        >
          <strong>{{ stat.homeValue }}</strong>

          <span>
            {{ translateStat(stat.type) }}
          </span>

          <strong>{{ stat.awayValue }}</strong>
        </div>
      </div>

      <div v-else class="info-box">
        Brak statystyk dla tego meczu.
      </div>
    </div>
  </div>
</main>
</template>

<script>
import api from '../api/axios'

export default {
  name: 'FixturesView',

  data() {
    return {
      fixtures: [],
      standings: [],
      staff: null,
      loading: true,
      standingsLoading: true,
      selectedMatch: null,
      selectedMatchStats: null,
      statsLoading: false
    }
  },

  computed: {
    lastResults() {
      return this.fixtures
        .filter(match => match.homeGoals !== null && match.awayGoals !== null)
        .sort((a, b) => new Date(b.date) - new Date(a.date))
    },

    visibleStandings() {
      if (!this.staff || this.standings.length === 0) return []

      const teamName = this.normalizeName(this.staff.team)

      const teamIndex = this.standings.findIndex(team => {
        const standingTeamName = this.normalizeName(team.teamName)

        return (
          standingTeamName.includes(teamName) ||
          teamName.includes(standingTeamName)
        )
      })

      if (teamIndex === -1) {
        return this.standings.slice(0, 10)
      }

      let start = teamIndex - 3
      let end = teamIndex + 7

      if (start < 0) {
        start = 0
        end = 10
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

    await Promise.all([
      this.fetchFixtures(),
      this.fetchStandings()
    ])
  },

  methods: {
    async fetchStaff() {
      const response = await api.get('/Staff/me')
      this.staff = response.data
    },

    async fetchFixtures() {
      try {
        const response = await api.get('/Fixtures/my-team-premier-league-season')
        this.fixtures = response.data
      } finally {
        this.loading = false
      }
    },

    getScoreClass(match) {
  if (!this.staff) return ''

  const myTeam = this.normalizeName(this.staff.team)
  const home = this.normalizeName(match.homeTeamName)
  const away = this.normalizeName(match.awayTeamName)

  const isHome = home.includes(myTeam) || myTeam.includes(home)
  const isAway = away.includes(myTeam) || myTeam.includes(away)

  if (!isHome && !isAway) return ''

  if (match.homeGoals === match.awayGoals) {
    return 'draw-score'
  }

  const myTeamWon =
    (isHome && match.homeGoals > match.awayGoals) ||
    (isAway && match.awayGoals > match.homeGoals)

  return myTeamWon ? 'win-score' : 'loss-score'
},

translateStat(type) {
  const map = {
    'Shots on Goal': 'Strzały celne',
    'Shots off Goal': 'Strzały niecelne',
    'Total Shots': 'Strzały',
    'Blocked Shots': 'Strzały zablokowane',
    'Shots insidebox': 'Strzały z pola karnego',
    'Shots outsidebox': 'Strzały zza pola karnego',
    'Fouls': 'Faule',
    'Corner Kicks': 'Rzuty rożne',
    'Offsides': 'Spalone',
    'Ball Possession': 'Posiadanie piłki',
    'Yellow Cards': 'Żółte kartki',
    'Red Cards': 'Czerwone kartki',
    'Goalkeeper Saves': 'Interwencje bramkarza',
    'Total passes': 'Podania',
    'Passes accurate': 'Celne podania',
    'Passes %': 'Celność podań'
  }

  return map[type] || type
},

    async fetchStandings() {
      try {
        const response = await api.get('/Fixtures/premier-league-standings')
        this.standings = response.data
      } finally {
        this.standingsLoading = false
      }
    },

    async openMatchDetails(match) {
  this.selectedMatch = match
  this.selectedMatchStats = null
  this.statsLoading = true

  try {
    const response = await api.get(`/Fixtures/${match.fixtureId}/statistics`)
    this.selectedMatchStats = response.data
  } finally {
    this.statsLoading = false
  }
},

    formatRound(round) {
      if (!round) return '?'

      const match = round.toString().match(/\d+/g)
      return match ? match[match.length - 1] : round
    },

    normalizeName(name) {
      return name
        ?.toLowerCase()
        .replaceAll('&', 'and')
        .replaceAll('fc', '')
        .replaceAll('afc', '')
        .replaceAll('the', '')
        .trim()
    },

    isMyTeam(teamName) {
      if (!this.staff) return false

      const staffTeam = this.normalizeName(this.staff.team)
      const tableTeam = this.normalizeName(teamName)

      return tableTeam.includes(staffTeam) || staffTeam.includes(tableTeam)
    },

    getTableRowClass(rank) {
      if (rank === 1) return 'rank-first'
      if (rank >= 2 && rank <= 5) return 'rank-top'
      if (rank >= 18 && rank <= 20) return 'rank-relegation'
      return ''
    },

    formatDate(date) {
      return new Date(date).toLocaleDateString('pl-PL', {
        day: '2-digit',
        month: 'long',
        year: 'numeric'
      })
    }
  }
}
</script>

<style scoped>
.fixtures-page {
  min-height: 100vh;
  padding: 40px 38px 40px;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
  color: white;
  font-family: 'PremierLeague', sans-serif;
}

.fixtures-layout {
  display: grid;
  grid-template-columns: 42% 58%;
  gap: 42px;
  max-width: 1450px;
  margin: 0 auto;
}

.section-header {
  margin-bottom: 28px;
}

.section-header h1 {
  margin: 0;
  color: white;
  font-size: 4.4rem;
  font-weight: 800;
  line-height: 1.05;
}

.header-line {
  margin-top: 8px;
  width: 520px;
  max-width: 100%;
  height: 4px;
  background: rgba(255, 255, 255, 0.95);
  border-radius: 999px;
}

.results-scroll {
  max-height: 680px;
  overflow-y: auto;
  padding-right: 26px;
}

.results-scroll::-webkit-scrollbar {
  width: 8px;
}

.results-scroll::-webkit-scrollbar-track {
  margin-top: 18px;
  margin-bottom: 18px;
  background: rgba(255, 255, 255, 0.08);
  border-radius: 999px;
}

.results-scroll::-webkit-scrollbar-thumb {
  background: #8806d4c7;
  border-radius: 999px;
}

.result-card {
  margin-bottom: 28px;
}

.result-card h2 {
  font-size: 30px;
  margin-bottom: 10px;
}

.results-box {
  padding: 0px 22px 22px;
}

.match-box {
  background: rgba(116, 20, 169, 0.72);
  border-radius: 24px;
  padding: 16px 22px;
  box-shadow: 0 10px 22px rgba(0, 0, 0, 0.22);
}

.fixture-style {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 95px minmax(0, 1fr);
  align-items: center;
  gap: 14px;
}

.team-side {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 22px;
  font-weight: 800;
}

.team-side img {
  width: 38px;
  height: 38px;
  object-fit: contain;
}

.away-side {
  justify-content: flex-end;
  text-align: right;
}

.score-center {
  width: 95px;
  text-align: center;
  font-size: 28px;
  font-weight: 900;
}

.home-side {
  justify-content: flex-start;
}

.away-side {
  justify-content: flex-end;
  text-align: right;
}

.date {
  margin-top: 8px;
  opacity: 0.8;
  font-size: 15px;
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

.standings-section {
  padding-top: 6px;
}

.content-box {
  background: #3b0c7281;
  border-radius: 28px;
  color: white;
  box-shadow: 0 14px 30px rgba(0, 0, 0, 0.16);
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
  overflow: hidden;
  border-radius: 18px;
}

.standings-table thead {
  background: rgba(255, 255, 255, 0.08);
}

.standings-table th {
  text-align: left;
  opacity: 0.8;
  font-weight: 600;
  padding: 15px 12px;
  font-size: 17px;
}

.standings-table td {
  padding: 15px 12px;
  text-align: left;
  font-size: 17px;
}

.standings-table tbody tr {
  background: rgba(45, 38, 125, 0.8);
  border-bottom: 1px solid rgba(255, 255, 255, 0.08);
}

.team-cell {
  display: flex;
  align-items: center;
  gap: 8px;
}

.team-cell img {
  width: 35px;
  height: 35px;
  object-fit: contain;
}

.match-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.details-button {
  border: none;
  background: rgba(255, 255, 255, 0.14);
  color: white;
  padding: 7px 14px;
  border-radius: 999px;
  font-family: 'PremierLeague', sans-serif;
  font-weight: 800;
  cursor: pointer;
  transition: 0.2s ease;
}

.details-button:hover {
  background: rgba(255, 255, 255, 0.26);
  transform: translateY(-1px);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  z-index: 999;
  background: rgba(10, 0, 20, 0.72);
  backdrop-filter: blur(5px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 32px;
}

.match-modal {
  position: relative;
  width: min(950px, 95vw);
  max-height: 90vh;
  overflow-y: auto;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
  border-radius: 34px;
  padding: 34px;
  color: white;
  box-shadow: 0 25px 80px rgba(0, 0, 0, 0.45);
}

.modal-close {
  position: absolute;
  top: 18px;
  right: 22px;
  border: none;
  background: rgba(255, 255, 255, 0.14);
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  font-size: 28px;
  cursor: pointer;
}

.match-modal h2 {
  font-size: 36px;
  margin: 0;
}

.modal-date {
  opacity: 0.8;
  margin: 8px 0 26px;
}

.modal-score {
  display: grid;
  grid-template-columns: 1fr 140px 1fr;
  align-items: center;
  gap: 22px;
  background: rgba(255, 255, 255, 0.08);
  border-radius: 26px;
  padding: 24px;
  margin-bottom: 26px;
}

.modal-score strong {
  text-align: center;
  font-size: 42px;
  font-weight: 900;
}

.modal-team {
  display: flex;
  align-items: center;
  gap: 16px;
  font-size: 26px;
  font-weight: 900;
}

.modal-team.right {
  justify-content: flex-end;
  text-align: right;
}

.modal-team img {
  width: 58px;
  height: 58px;
  object-fit: contain;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 16px;
  margin-bottom: 22px;
}

.stat-box {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 18px;
}

.stat-box span {
  display: block;
  opacity: 0.7;
  margin-bottom: 8px;
}

.stat-box strong {
  font-size: 20px;
}

.points {
  font-weight: 800;
}

.win-score {
  color: #35d84b;
}

.loss-score {
  color: #ff4d4d;
}

.draw-score {
  color: #b8b8b8;
}

.rank-first {
  background: rgba(21, 224, 28, 0.36) !important;
}

.rank-top {
  background: rgba(26, 113, 184, 0.32) !important;
}

.rank-relegation {
  background: rgba(243, 18, 18, 0.35) !important;
}

.my-team-row td {
  font-weight: 900;
  background: rgba(114, 42, 209, 0.48);
}

.info-box {
  background: rgba(255, 255, 255, 0.12);
  padding: 24px;
  border-radius: 20px;
  font-size: 22px;
}

.stats-list {
  display: grid;
  gap: 10px;
}

.stat-row {
  display: grid;
  grid-template-columns: 90px 1fr 90px;
  align-items: center;
  background: rgba(255, 255, 255, 0.09);
  border-radius: 16px;
  padding: 12px 16px;
}

.stat-row span {
  text-align: center;
  opacity: 0.85;
}

.stat-row strong {
  font-size: 20px;
  text-align: center;
}

@media (max-width: 1000px) {
  .fixtures-layout {
    grid-template-columns: 1fr;
  }

  .fixtures-page {
    padding: 110px 22px 35px;
  }
}
</style>