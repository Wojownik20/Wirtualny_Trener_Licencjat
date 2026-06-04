<template>
  <main class="statistics-page">
    <section class="statistics-container">
      <div v-if="loading" class="info-box">
        Ładowanie statystyk...
      </div>

      <div v-else-if="error" class="info-box error-box">
        {{ error }}
      </div>

      <template v-else-if="stats">
        
        <section class="stats-hero">
          <section class="team-picker-box">
    <label>Wybierz zespół:</label>

    <select v-model="selectedTeamId" @change="changeTeam">
      <option :value="null">Mój zespół</option>

      <option
        v-for="team in teams"
        :key="team.id"
        :value="team.id"
      >
        {{ team.name }}
      </option>
    </select>
  </section>
          <div class="hero-left">
            <img
              v-if="stats.team?.teamLogo"
              :src="stats.team.teamLogo"
              alt=""
              class="team-logo"
            />

            <div>
              <h1>Statystyki sezonu</h1>
              <div class="header-line"></div>
              <p>
                {{ stats.team.teamName }} • {{ stats.team.leagueName }}
                {{ stats.team.season }}/{{ stats.team.season + 1 }}
              </p>
            </div>
          </div>

          <div class="league-position">
            <span>Miejsce</span>
            <strong>#{{ stats.leagueStanding?.rank || '-' }}</strong>
          </div>
        </section>

        <section class="summary-grid">
          <div class="summary-card big">
            <span>Punkty</span>
            <strong>{{ stats.seasonSummary.points }}</strong>
          </div>

          <div class="summary-card">
            <span>Mecze</span>
            <strong>{{ stats.seasonSummary.played }}</strong>
          </div>

          <div class="summary-card">
            <span>Zwycięstwa</span>
            <strong>{{ stats.seasonSummary.wins }}</strong>
          </div>

          <div class="summary-card">
            <span>Remisy</span>
            <strong>{{ stats.seasonSummary.draws }}</strong>
          </div>

          <div class="summary-card">
            <span>Porażki</span>
            <strong>{{ stats.seasonSummary.losses }}</strong>
          </div>

          <div class="summary-card">
            <span>Bilans bramkowy</span>
            <strong>{{ stats.seasonSummary.goalDifference }}</strong>
          </div>
        </section>

        <section class="details-grid">
          <div class="content-box">
            <h2>Bramki</h2>

            <div class="goals-row">
              <div>
                <span>Strzelone</span>
                <strong>{{ stats.seasonSummary.goalsFor }}</strong>
              </div>

              <div>
                <span>Stracone</span>
                <strong>{{ stats.seasonSummary.goalsAgainst }}</strong>
              </div>

              <div>
                <span>Śr. strzelone</span>
                <strong>{{ stats.seasonSummary.averageGoalsFor }}</strong>
              </div>

              <div>
                <span>Śr. stracone</span>
                <strong>{{ stats.seasonSummary.averageGoalsAgainst }}</strong>
              </div>
            </div>

            <div class="mini-info">
              <p>Największa wygrana: <b>{{ stats.seasonSummary.biggestWin || '-' }}</b></p>
              <p>Największa porażka: <b>{{ stats.seasonSummary.biggestLoss || '-' }}</b></p>
            </div>
          </div>

          <div class="content-box">
            <h2>Forma</h2>

            <div class="form-row">
              <span
  v-for="(result, index) in [...stats.form.lastTenResults].reverse()"
  :key="index"
  class="form-badge"
  :class="resultClass(result)"
>
  {{ result }}
</span>
            </div>

            <div class="streak-grid">
              <div>
                <span>Seria zwycięstw</span>
                <strong>{{ stats.form.currentWinStreak }}</strong>
              </div>

              <div>
                <span>Bez porażki</span>
                <strong>{{ stats.form.currentUnbeatenStreak }}</strong>
              </div>

              <div>
                <span>Najdłużej bez porażki</span>
                <strong>{{ stats.form.longestUnbeatenStreak }}</strong>
              </div>
            </div>
          </div>
        </section>

        <section class="content-box">
          <h2>Dom vs Wyjazd</h2>

          <div class="home-away-grid">
            <div class="home-away-card">
              <h3>U siebie</h3>

              <div class="mini-stats">
                <p>Mecze <b>{{ stats.homeStats.played }}</b></p>
                <p>W/R/P <b>{{ stats.homeStats.wins }}/{{ stats.homeStats.draws }}/{{ stats.homeStats.losses }}</b></p>
                <p>Bramki <b>{{ stats.homeStats.goalsFor }}:{{ stats.homeStats.goalsAgainst }}</b></p>
                <p>Czyste konta <b>{{ stats.homeStats.cleanSheets }}</b></p>
              </div>
            </div>

            <div class="home-away-card">
              <h3>Na wyjeździe</h3>

              <div class="mini-stats">
                <p>Mecze <b>{{ stats.awayStats.played }}</b></p>
                <p>W/R/P <b>{{ stats.awayStats.wins }}/{{ stats.awayStats.draws }}/{{ stats.awayStats.losses }}</b></p>
                <p>Bramki <b>{{ stats.awayStats.goalsFor }}:{{ stats.awayStats.goalsAgainst }}</b></p>
                <p>Czyste konta <b>{{ stats.awayStats.cleanSheets }}</b></p>
              </div>
            </div>
          </div>
        </section>

        <section class="content-box">
          <div class="table-header">
            <h2>Statystyki zawodników</h2>

            <select v-model="sortBy">
              <option value="default">Domyślne</option>
              <option value="minutes">Minuty</option>
              <option value="goals">Gole</option>
              <option value="assists">Asysty</option>
              <option value="rating">Ocena</option>
              <option value="yellowCards">Kartki</option>
            </select>
          </div>

          <div class="players-table-wrapper">
            <table class="players-table">
              <thead>
                <tr>
                  <th>Zawodnik</th>
                  <th>Pozycja</th>
                  <th>Mecze</th>
                  <th>Minuty</th>
                  <th>Gole</th>
                  <th>Asysty</th>
                  <th>Ocena</th>
                  <th>ŻK</th>
                </tr>
              </thead>

              <tbody>
                <tr
                  v-for="player in sortedPlayers"
                  :key="player.externalPlayerId"
                >
                  <td class="player-cell">
                    <img
                      v-if="player.photo"
                      :src="player.photo"
                      alt=""
                    />
                    <span>{{ player.name }}</span>
                  </td>
                  <td>{{ translatePosition(player.position) }}</td>
                  <td>{{ player.appearances }}</td>
                  <td>{{ player.minutes }}</td>
                  <td>{{ player.goals }}</td>
                  <td>{{ player.assists }}</td>
                  <td>{{ player.rating || '-' }}</td>
                  <td>{{ player.yellowCards }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </template>
    </section>
  </main>
</template>

<script>
import api from '../api/axios'

const PlayerLeaderCard = {
  props: {
    title: String,
    player: Object,
    statLabel: String,
    statKey: String
  }
}

export default {
  name: 'StatisticsView',

  components: {
    PlayerLeaderCard
  },

  data() {
  return {
    stats: null,
    teams: [],
    selectedTeamId: null,
    loading: true,
    teamsLoading: false,
    error: null,
    sortBy: 'default'
  }
},

  computed: {
    sortedPlayers() {
  if (!this.stats?.players) return []

  const positionOrder = {
    Goalkeeper: 1,
    Defender: 2,
    Midfielder: 3,
    Attacker: 4
  }

  if (this.sortBy === 'default') {
    return [...this.stats.players].sort((a, b) => {
      const posA = positionOrder[a.position] || 99
      const posB = positionOrder[b.position] || 99

      if (posA !== posB) {
        return posA - posB
      }

      return (a.number || 999) - (b.number || 999)
    })
  }

  return [...this.stats.players].sort((a, b) => {
    const first = b[this.sortBy] || 0
    const second = a[this.sortBy] || 0
    return first - second
  })
}
  },

  async mounted() {
  await this.fetchTeams()
  await this.fetchStats()
},

  methods: {

    async fetchTeams() {
  try {
    this.teamsLoading = true

    const response = await api.get('/Teams')
    this.teams = response.data.$values || response.data || []
  } catch (error) {
    console.error('Błąd pobierania drużyn:', error)
  } finally {
    this.teamsLoading = false
  }
},

    async fetchStats() {
  try {
    this.loading = true
    this.error = null

    const endpoint = this.selectedTeamId
      ? `/Statistics/team/${this.selectedTeamId}/full`
      : '/Statistics/my-team-full'

    const response = await api.get(endpoint)
    this.stats = response.data
  } catch (error) {
    this.error = 'Nie udało się pobrać statystyk.'
  } finally {
    this.loading = false
  }
},

async changeTeam() {
  this.sortBy = 'default'
  await this.fetchStats()
},

    resultClass(result) {
      if (result === 'W') return 'win'
      if (result === 'D') return 'draw'
      if (result === 'L') return 'loss'
      return ''
    },

    translatePosition(position) {
      const map = {
        Goalkeeper: 'Bramkarz',
        Defender: 'Obrońca',
        Midfielder: 'Pomocnik',
        Attacker: 'Napastnik'
      }

      return map[position] || position || '-'
    }
  }
}
</script>

<style scoped>
.statistics-page {
  min-height: 100vh;
  padding: 40px 40px 70px;
  color: white;
  font-family: 'PremierLeague', sans-serif;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
}

.statistics-container {
  max-width: 1450px;
  margin: 0 auto;
}

.info-box {
  margin-top: 80px;
  background: rgba(59, 12, 114, 0.6);
  border-radius: 26px;
  padding: 35px;
  text-align: center;
  font-size: 1.6rem;
  font-weight: 700;
}

.error-box {
  background: rgba(215, 25, 32, 0.55);
}

.stats-hero {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(59, 12, 114, 0.55);
  border-radius: 32px;
  padding: 35px 42px;
  margin-bottom: 30px;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.22);
}

.hero-left {
  display: flex;
  align-items: center;
  gap: 24px;
}

.team-logo {
  width: 96px;
  height: 96px;
  object-fit: contain;
}

.stats-hero h1 {
  font-size: 4.2rem;
  margin: 0;
  font-weight: 800;
}

.header-line {
  width: 620px;
  height: 5px;
  background: white;
  border-radius: 999px;
  margin: 10px 0 12px;
}

.stats-hero p {
  font-size: 1.35rem;
  margin: 0;
  opacity: 0.9;
}

.league-position {
  background: rgba(255, 255, 255, 0.12);
  border-radius: 26px;
  padding: 20px 34px;
  text-align: center;
}

.league-position span {
  display: block;
  font-size: 1.1rem;
  opacity: 0.85;
}

.league-position strong {
  font-size: 3.4rem;
  line-height: 1;
}

.summary-grid {
  display: grid;
  grid-template-columns: 1.4fr repeat(5, 1fr);
  gap: 18px;
  margin-bottom: 24px;
}

.summary-card {
  background: rgba(167, 22, 46, 0.425);
  border-radius: 28px;
  padding: 28px 24px;
  min-height: 130px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  box-shadow: 0 14px 32px rgba(0, 0, 0, 0.2);
}

.summary-card.big {
  background: linear-gradient(135deg, #eb5c61a1, #ff3b44);
}

.summary-card span {
  font-size: 1.1rem;
  opacity: 0.9;
}

.summary-card strong {
  font-size: 3.2rem;
  line-height: 1;
}

.content-box {
  background: rgba(59, 12, 114, 0.58);
  border-radius: 30px;
  padding: 30px;
  margin-bottom: 24px;
  box-shadow: 0 14px 34px rgba(0, 0, 0, 0.18);
}

.content-box h2 {
  font-size: 2.1rem;
  margin: 0 0 24px;
}

.details-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
}

.goals-row,
.streak-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.goals-row div,
.streak-grid div {
  background: rgba(255, 255, 255, 0.12);
  border-radius: 22px;
  padding: 20px;
}

.goals-row span,
.streak-grid span {
  display: block;
  opacity: 0.82;
  font-size: 1rem;
}

.goals-row strong,
.streak-grid strong {
  display: block;
  margin-top: 8px;
  font-size: 2rem;
}

.mini-info {
  margin-top: 22px;
  font-size: 1.1rem;
}

.mini-info p {
  margin: 8px 0;
}

.form-row {
  display: flex;
  gap: 12px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.form-badge {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: grid;
  place-items: center;
  font-size: 1.2rem;
  font-weight: 900;
}

.form-badge.win {
  background: #17b26a;
}

.form-badge.draw {
  background: #f7b731;
}

.form-badge.loss {
  background: #d71920;
}

.home-away-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
}

.home-away-card {
  background: rgba(255, 255, 255, 0.12);
  border-radius: 26px;
  padding: 26px;
}

.home-away-card h3 {
  font-size: 1.8rem;
  margin: 0 0 18px;
}

.mini-stats p {
  display: flex;
  justify-content: space-between;
  margin: 12px 0;
  font-size: 1.2rem;
}

.leaders-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

:deep(.leader-card) {
  background: #d71920;
  border-radius: 28px;
  padding: 22px;
  min-height: 210px;
}

:deep(.leader-title) {
  display: block;
  font-size: 1rem;
  opacity: 0.9;
  margin-bottom: 18px;
}

:deep(.leader-body) {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
}

:deep(.leader-body img) {
  width: 76px;
  height: 76px;
  border-radius: 50%;
  object-fit: cover;
  background: white;
  margin-bottom: 12px;
}

:deep(.leader-info strong) {
  display: block;
  font-size: 1.25rem;
}

:deep(.leader-info small) {
  opacity: 0.85;
}

:deep(.leader-stat) {
  margin-top: 16px;
}

:deep(.leader-stat span) {
  display: block;
  opacity: 0.85;
}

:deep(.leader-stat b) {
  font-size: 2rem;
}

:deep(.leader-empty) {
  opacity: 0.8;
  text-align: center;
  margin-top: 50px;
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-header select {
  background: #d71920;
  color: white;
  border: none;
  border-radius: 16px;
  padding: 12px 18px;
  font-weight: 800;
  outline: none;
}

.players-table-wrapper {
  overflow-x: auto;
  overflow-y: auto;
  max-height: 620px;
  padding-right: 6px;
  border-radius: 18px;
}

.players-table-wrapper::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

.players-table-wrapper::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.322);
  border-radius: 999px;
}

.players-table-wrapper::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.164);
  border-radius: 999px;
}

.players-table thead {
  position: sticky;
  top: 0;
  z-index: 5;
  background: #4b0b6d;
}

.players-table thead th {
  background: #4b0b6d;
}

.players-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 900px;
}

.players-table th {
  text-align: left;
  padding: 16px;
  opacity: 0.8;
  font-size: 1rem;
}

.players-table td {
  padding: 15px 16px;
  border-top: 1px solid rgba(255, 255, 255, 0.14);
  font-size: 1.05rem;
}

.player-cell {
  display: flex;
  align-items: center;
  gap: 12px;
  font-weight: 800;
}

.player-cell img {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  background: white;
  object-fit: cover;
}

.team-picker-box {
  position: absolute;
  top: 80px;
  right: 230px;
}

.team-picker-box label {
  display: none;
}

.team-picker-box select {
  background: #5f108a;
  color: rgb(255, 255, 255);

  border: 1px solid rgba(255,255,255,0.15);
  border-radius: 999px;

  padding: 6px 14px;

  font-size: 0.85rem;
  font-weight: 700;

  min-width: 140px;
}

@media (max-width: 1200px) {
  .summary-grid,
  .details-grid,
  .home-away-grid,
  .leaders-grid {
    grid-template-columns: 1fr 1fr;
  }

  .summary-card.big {
    grid-column: span 2;
  }
}

@media (max-width: 750px) {
  .statistics-page {
    padding: 25px 18px 50px;
  }

  .stats-hero,
  .hero-left {
    flex-direction: column;
    text-align: center;
  }

  .team-picker-box {
  flex-direction: column;
  align-items: stretch;
}

.team-picker-box select {
  width: 100%;
}

  .stats-hero h1 {
    font-size: 3rem;
  }

  .header-line {
    width: 260px;
    margin-left: auto;
    margin-right: auto;
  }

  .summary-grid,
  .details-grid,
  .home-away-grid,
  .leaders-grid,
  .goals-row,
  .streak-grid {
    grid-template-columns: 1fr;
  }

  .summary-card.big {
    grid-column: auto;
  }
}
</style>