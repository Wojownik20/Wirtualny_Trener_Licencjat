<template>
  <main class="players-page">
    <section class="players-container">
      <div class="section-header">
        <h1>Pełny skład</h1>
        <div class="header-line"></div>
        <p>Liczba piłkarzy: {{ uniquePlayersCount }}</p>
      </div>

      <div v-if="loading" class="info-box">
        Ładowanie składu...
      </div>

      <div v-else-if="players.length === 0" class="info-box">
        Brak zawodników w składzie.
      </div>

      <div v-else class="squad-box">
        <section
          v-for="group in groupedPlayers"
          :key="group.title"
          class="position-section"
        >
          <h2>{{ group.title }} :</h2>

          <div class="players-grid">
  <div
    v-for="player in group.players"
    :key="player.id"
    class="player-card"
  >
    <img
  v-if="player.photo"
  class="player-photo"
  :src="player.photo"
  :alt="getPlayerName(player)"
/>

<div v-else class="player-avatar">
  {{ getInitials(player) }}
</div>

    <div class="player-info">
      <h3>{{ getPlayerName(player) }}</h3>
      <p>#{{ player.shirtNumber || player.number || '?' }}</p>
      <span>{{ formatPosition(player.position) }}</span>
    </div>

    <img
  class="player-flag"
  :src="getFlagUrl(player.nationality)"
  :alt="player.nationality"
/>

<button class="more-btn" @click="openPlayerModal(player)">
  Więcej
</button>
  </div>
</div>
        </section>
      </div>
    </section>

    <div v-if="selectedPlayer" class="player-modal-overlay">
  <div class="player-modal">
    <button class="modal-close" @click="selectedPlayer = null">×</button>

    <div class="modal-header">
      <img
  v-if="selectedPlayer.photo"
  class="modal-photo"
  :src="selectedPlayer.photo"
  :alt="getPlayerName(selectedPlayer)"
/>

<div v-else class="modal-avatar">
  {{ getInitials(selectedPlayer) }}
</div>

      <div>
  <div class="player-title">
    <h2>{{ getPlayerName(selectedPlayer) }}</h2>

    <img
      class="title-flag"
      :src="getFlagUrl(selectedPlayer.nationality)"
      :alt="selectedPlayer.nationality"
    />
  </div>

  <p>#{{ selectedPlayer.number || selectedPlayer.shirtNumber || '?' }} • {{ selectedPlayer.position }}</p>
</div>
    </div>

    <div class="player-details-grid">
      <div class="detail-box">
        <span>Wiek</span>
        <strong>{{ selectedPlayer.age || 'Brak danych' }}</strong>
      </div>

      <div class="detail-box">
        <span>Narodowość</span>
        <strong>{{ selectedPlayer.nationality || 'Brak danych' }}</strong>
      </div>

      <div class="detail-box">
  <span>Status zawodnika</span>

 <div class="custom-status-dropdown">
  <div
  class="selected-status"
  :class="{
    'selected-healthy': editedInjuryStatus === 'Zdrowy',
    'selected-injured': editedInjuryStatus === 'Kontuzjowany',
    'selected-excluded': editedInjuryStatus === 'Wykluczony'
  }"
  @click="statusDropdownOpen = !statusDropdownOpen"
>
    <span>{{ editedInjuryStatus }}</span>

    <span
      class="dropdown-arrow"
      :class="{ open: statusDropdownOpen }"
    >
      ▼
    </span>
  </div>

  <div
    v-if="statusDropdownOpen"
    class="status-options"
  >
    <div
  class="status-option healthy-option"
  :class="{ active: editedInjuryStatus === 'Zdrowy' }"
  @click="selectStatus('Zdrowy')"
>
  Zdrowy
</div>

<div
  class="status-option injured-option"
  :class="{ active: editedInjuryStatus === 'Kontuzjowany' }"
  @click="selectStatus('Kontuzjowany')"
>
  Kontuzjowany
</div>

<div
  class="status-option excluded-option"
  :class="{ active: editedInjuryStatus === 'Wykluczony' }"
  @click="selectStatus('Wykluczony')"
>
  Wykluczony
</div>
  </div>
</div>
</div>
    </div>

    <div class="notes-box">
  <div class="notes-header">
    <span>Notatki sztabu</span>
  </div>

  <textarea
    v-model="editedNotes"
    class="notes-textarea"
    placeholder="Dodaj notatkę o zawodniku..."
  ></textarea>

  <div class="notes-actions">
  <button
    class="save-notes-btn"
    :disabled="savingNotes"
    @click="savePlayerNotes"
  >
    {{ savingNotes ? 'Zapisywanie...' : 'Zapisz notatkę' }}
  </button>

  <div v-if="saveSuccess" class="save-success">
    ✓ Zapisano
  </div>
</div>
</div>

<div class="live-stats-grid">
  <div class="detail-box">
    <span>Występy</span>
    <strong>{{ selectedPlayer.appearances || 0 }}</strong>
  </div>

  <div class="detail-box">
    <span>Minuty</span>
    <strong>{{ selectedPlayer.minutes || 0 }}</strong>
  </div>

  <div class="detail-box">
    <span>Ocena</span>
    <strong>{{ selectedPlayer.rating || '-' }}</strong>
  </div>

  <div class="detail-box">
    <span>Gole</span>
    <strong>{{ selectedPlayer.goals || 0 }}</strong>
  </div>

  <div class="detail-box">
    <span>Asysty</span>
    <strong>{{ selectedPlayer.assists || 0 }}</strong>
  </div>

  <div class="detail-box">
    <span>Kluczowe podania</span>
    <strong>{{ selectedPlayer.keyPasses || 0 }}</strong>
  </div>

  <div class="detail-box">
    <span>Celne strzały</span>
    <strong>{{ selectedPlayer.shotsOnTarget || 0 }}</strong>
  </div>

  <div class="detail-box">
    <span>Wygrane pojedynki</span>
    <strong>{{ selectedPlayer.duelsWon || 0 }}</strong>
  </div>
</div>
    
  </div>
</div>
  </main>
</template>

<script>
import api from '../api/axios'

export default {
  name: 'PlayersView',

  data() {
    return {
      players: [],
      loading: true,
      selectedPlayer: null,
      editedNotes: '',
savingNotes: false,
saveSuccess: false,
statusDropdownOpen: false,
editedInjuryStatus: 'Zdrowy'
    }
  },

  computed: {
groupedPlayers() {
  const uniquePlayers = Array.from(
    new Map(this.players.map(p => [p.externalPlayerId || p.id || p.name, p])).values()
  )

  const getPos = p => this.normalizePosition(
    p.position || p.playerPosition || p.role || p.Position || ''
  )

  const groups = [
    {
      title: 'Bramkarze',
      players: uniquePlayers.filter(p =>
        ['goalkeeper', 'goal keeper', 'gk', 'bramkarz'].some(x => getPos(p).includes(x))
      )
    },
    {
      title: 'Obrońcy',
      players: uniquePlayers.filter(p =>
        ['defender', 'defence', 'defense', 'cb', 'rb', 'lb', 'rwb', 'lwb', 'obrońca', 'obronca'].some(x => getPos(p).includes(x))
      )
    },
    {
      title: 'Pomocnicy',
      players: uniquePlayers.filter(p =>
        ['midfielder', 'midfield', 'cm', 'cdm', 'dm', 'cam', 'am', 'lm', 'rm', 'pomocnik'].some(x => getPos(p).includes(x))
      )
    },
    {
      title: 'Napastnicy',
      players: uniquePlayers.filter(p =>
        ['attacker', 'forward', 'striker', 'lw', 'rw', 'st', 'cf', 'napastnik'].some(x => getPos(p).includes(x))
      )
    }
  ]

  const groupedIds = groups.flatMap(g =>
    g.players.map(p => p.externalPlayerId || p.id || p.name)
  )

  const others = uniquePlayers.filter(p =>
    !groupedIds.includes(p.externalPlayerId || p.id || p.name)
  )

  if (others.length > 0) {
    groups.push({
      title: 'Pozostali',
      players: others
    })
  }

  return groups.filter(group => group.players.length > 0)
},
uniquePlayersCount() {
  return new Map(
    this.players.map(p => [p.externalPlayerId || p.id || p.name, p])
  ).size
}


    },

  async mounted() {
    await this.fetchPlayers()
  },

  methods: {
    async fetchPlayers() {
      try {
        this.loading = true

        const response = await api.get('/players/my-team-live')

        this.players = response.data.$values || response.data || []
      } catch (error) {
        console.error('Błąd pobierania składu:', error)
      } finally {
        this.loading = false
      }
    },formatPosition(position) {
  const map = {
    Goalkeeper: 'Bramkarz',
    Defender: 'Obrońca',
    Midfielder: 'Pomocnik',
    Attacker: 'Napastnik'
  }

  return map[position] || position || 'Brak pozycji'
},

selectStatus(status) {
  this.editedInjuryStatus = status
  this.statusDropdownOpen = false
},

openPlayerModal(player) {
  this.selectedPlayer = player
  this.editedNotes = player.notes || ''
  this.editedInjuryStatus = player.injuryStatus || 'Zdrowy'
},

async savePlayerNotes() {
  if (!this.selectedPlayer) return

  try {
    this.savingNotes = true

    await api.put(`/players/external/${this.selectedPlayer.externalPlayerId}/staff-data`, {
  injuryStatus: this.editedInjuryStatus,
  notes: this.editedNotes
})

    this.selectedPlayer.notes = this.editedNotes
    this.saveSuccess = true
    this.selectedPlayer.injuryStatus = this.editedInjuryStatus

setTimeout(() => {
  this.saveSuccess = false
}, 2000)

    const playerIndex = this.players.findIndex(p =>
      p.id === this.selectedPlayer.id ||
      p.externalPlayerId === this.selectedPlayer.externalPlayerId
    )

    if (playerIndex !== -1) {
      this.players[playerIndex].notes = this.editedNotes
    }
  } catch (error) {
    console.error('Błąd zapisywania notatki:', error)
    alert('Nie udało się zapisać notatki.')
  } finally {
    this.savingNotes = false
  }
},

getFlagUrl(nationality) {
  const key = String(nationality || '').trim().toUpperCase()

  const map = {
    ALBANIA: 'al', ALBANIAN: 'al', AL: 'al',
    ANDORRA: 'ad', ANDORRAN: 'ad', AD: 'ad',
    ARMENIA: 'am', ARMENIAN: 'am', AM: 'am',
    AUSTRIA: 'at', AUSTRIAN: 'at', AT: 'at',
    AZERBAIJAN: 'az', AZERBAIJANI: 'az', AZ: 'az',
    BELARUS: 'by', BELARUSIAN: 'by', BY: 'by',
    BELGIUM: 'be', BELGIAN: 'be', BE: 'be',
    BOSNIA: 'ba', BOSNIAN: 'ba', 'BOSNIA AND HERZEGOVINA': 'ba', BA: 'ba',
    BULGARIA: 'bg', BULGARIAN: 'bg', BG: 'bg',
    CROATIA: 'hr', CROATIAN: 'hr', HR: 'hr',
    CYPRUS: 'cy', CYPRIOT: 'cy', CY: 'cy',
    CZECHIA: 'cz', CZECH: 'cz', 'CZECH REPUBLIC': 'cz', CZ: 'cz',
    DENMARK: 'dk', DANISH: 'dk', DK: 'dk',
    ENGLAND: 'gb-eng', ENGLISH: 'gb-eng',
    ESTONIA: 'ee', ESTONIAN: 'ee', EE: 'ee',
    FINLAND: 'fi', FINNISH: 'fi', FI: 'fi',
    FRANCE: 'fr', FRENCH: 'fr', FR: 'fr',
    GEORGIA: 'ge', GEORGIAN: 'ge', GE: 'ge',
    GERMANY: 'de', GERMAN: 'de', DE: 'de',
    GREECE: 'gr', GREEK: 'gr', GR: 'gr',
    HUNGARY: 'hu', HUNGARIAN: 'hu', HU: 'hu',
    ICELAND: 'is', ICELANDIC: 'is', IS: 'is',
    IRELAND: 'ie', IRISH: 'ie', IE: 'ie',
    ITALY: 'it', ITALIAN: 'it', IT: 'it',
    KOSOVO: 'xk', KOSOVAN: 'xk', XK: 'xk',
    LATVIA: 'lv', LATVIAN: 'lv', LV: 'lv',
    LITHUANIA: 'lt', LITHUANIAN: 'lt', LT: 'lt',
    LUXEMBOURG: 'lu', LUXEMBOURGISH: 'lu', LU: 'lu',
    MALTA: 'mt', MALTESE: 'mt', MT: 'mt',
    MOLDOVA: 'md', MOLDOVAN: 'md', MD: 'md',
    MONACO: 'mc', MONEGASQUE: 'mc', MC: 'mc',
    MONTENEGRO: 'me', MONTENEGRIN: 'me', ME: 'me',
    NETHERLANDS: 'nl', DUTCH: 'nl', NL: 'nl',
    NORTH_MACEDONIA: 'mk', MACEDONIAN: 'mk', 'NORTH MACEDONIA': 'mk', MK: 'mk',
    NORTHERN_IRELAND: 'gb-nir', 'NORTHERN IRELAND': 'gb-nir',
    REPUBLIC_OF_IRELAND: 'ie', 'REPUBLIC OF IRELAND': 'ie',
    NORWAY: 'no', NORWEGIAN: 'no', NO: 'no',
    POLAND: 'pl', POLISH: 'pl', PL: 'pl',
    PORTUGAL: 'pt', PORTUGUESE: 'pt', PT: 'pt',
    ROMANIA: 'ro', ROMANIAN: 'ro', RO: 'ro',
    RUSSIA: 'ru', RUSSIAN: 'ru', RU: 'ru',
    SAN_MARINO: 'sm', 'SAN MARINO': 'sm', SAMMARINESE: 'sm', SM: 'sm',
    SCOTLAND: 'gb-sct', SCOTTISH: 'gb-sct',
    SERBIA: 'rs', SERBIAN: 'rs', RS: 'rs',
    SLOVAKIA: 'sk', SLOVAK: 'sk', SK: 'sk',
    SLOVENIA: 'si', SLOVENIAN: 'si', SI: 'si',
    SPAIN: 'es', SPANISH: 'es', ES: 'es',
    SWEDEN: 'se', SWEDISH: 'se', SE: 'se',
    SWITZERLAND: 'ch', SWISS: 'ch', CH: 'ch',
    TURKEY: 'tr', TURKISH: 'tr', TR: 'tr',
    UKRAINE: 'ua', UKRAINIAN: 'ua', UA: 'ua',
    WALES: 'gb-wls', WELSH: 'gb-wls',

    ARGENTINA: 'ar', ARGENTINIAN: 'ar', AR: 'ar',
    BOLIVIA: 'bo', BOLIVIAN: 'bo', BO: 'bo',
    BRAZIL: 'br', BRAZILIAN: 'br', BR: 'br',
    CANADA: 'ca', CANADIAN: 'ca', CA: 'ca',
    CHILE: 'cl', CHILEAN: 'cl', CL: 'cl',
    COLOMBIA: 'co', COLOMBIAN: 'co', CO: 'co',
    COSTA_RICA: 'cr', 'COSTA RICA': 'cr', COSTA_RICAN: 'cr', CR: 'cr',
    CUBA: 'cu', CUBAN: 'cu', CU: 'cu',
    ECUADOR: 'ec', ECUADORIAN: 'ec', EC: 'ec',
    MEXICO: 'mx', MEXICAN: 'mx', MX: 'mx',
    PARAGUAY: 'py', PARAGUAYAN: 'py', PY: 'py',
    PERU: 'pe', PERUVIAN: 'pe', PE: 'pe',
    USA: 'us', UNITED_STATES: 'us', 'UNITED STATES': 'us', AMERICAN: 'us', US: 'us',
    URUGUAY: 'uy', URUGUAYAN: 'uy', UY: 'uy',
    VENEZUELA: 've', VENEZUELAN: 've', VE: 've',

    ALGERIA: 'dz', ALGERIAN: 'dz', DZ: 'dz',
    ANGOLA: 'ao', ANGOLAN: 'ao', AO: 'ao',
    CAMEROON: 'cm', CAMEROONIAN: 'cm', CM: 'cm',
    CONGO_DR: 'cd', 'CONGO DR': 'cd',
    EGYPT: 'eg', EGYPTIAN: 'eg', EG: 'eg',
    GHANA: 'gh', GHANAIAN: 'gh', GH: 'gh',
    IVORY_COAST: 'ci', 'IVORY COAST': 'ci', IVOIRIAN: 'ci', CI: 'ci',
    KENYA: 'ke', KENYAN: 'ke', KE: 'ke',
    MOROCCO: 'ma', MOROCCAN: 'ma', MA: 'ma',
    NIGERIA: 'ng', NIGERIAN: 'ng', NG: 'ng',
    SENEGAL: 'sn', SENEGALESE: 'sn', SN: 'sn',
    SOUTH_AFRICA: 'za', 'SOUTH AFRICA': 'za', SOUTH_AFRICAN: 'za', ZA: 'za',
    TUNISIA: 'tn', TUNISIAN: 'tn', TN: 'tn',

    CHINA: 'cn', CHINESE: 'cn', CN: 'cn',
    INDIA: 'in', INDIAN: 'in', IN: 'in',
    INDONESIA: 'id', INDONESIAN: 'id', ID: 'id',
    IRAN: 'ir', IRANIAN: 'ir', IR: 'ir',
    IRAQ: 'iq', IRAQI: 'iq', IQ: 'iq',
    ISRAEL: 'il', ISRAELI: 'il', IL: 'il',
    JAPAN: 'jp', JAPANESE: 'jp', JP: 'jp',
    SOUTH_KOREA: 'kr', 'SOUTH KOREA': 'kr', KOREAN: 'kr', KR: 'kr',
    SAUDI_ARABIA: 'sa', 'SAUDI ARABIA': 'sa', SAUDI: 'sa', SAUDI_ARABIAN: 'sa', SA: 'sa',
    THAILAND: 'th', THAI: 'th', TH: 'th',
    VIETNAM: 'vn', VIETNAMESE: 'vn', VN: 'vn'
  }

  const flagCode = map[key]

  return flagCode
    ? `https://flagcdn.com/w40/${flagCode}.png`
    : 'https://flagcdn.com/w40/un.png'
},

    normalizePosition(position) {
      return String(position || '').toLowerCase().trim()
    },

    getPlayerName(player) {
    if (player.fullName) return player.fullName
if (player.name) return player.name
if (player.playerName) return player.playerName
if (player.Name) return player.Name

      const firstName = player.firstName || ''
      const lastName = player.lastName || ''

      return `${firstName} ${lastName}`.trim() || 'Nieznany zawodnik'
    },

    getInitials(player) {
      const name = this.getPlayerName(player)
      return name
        .split(' ')
        .map(part => part[0])
        .join('')
        .slice(0, 2)
        .toUpperCase()
    },
}
}
</script>

<style scoped>
.players-page {
  min-height: 100vh;
  background: linear-gradient(180deg, #5b0b739c 0%, #32003f 100%);
  color: white;
  padding: 40px 40px 60px;
  font-family: 'PremierLeague', sans-serif;
}

.players-container {
  max-width: 1200px;
  margin: 0 auto;
}

.section-header {
  margin-bottom: 35px;
}

.section-header h1 {
  margin: 0;
  color: white;
  font-size: 4.4rem;
  font-weight: 800;
  line-height: 1.05;
}

.header-line {
  width: 415px;
  height: 5px;
  background: white;
  border-radius: 20px;
  margin: 14px 0 16px;
}

.section-header p {
  font-size: 24px;
  margin: 0;
}

.info-box {
  background: rgba(255, 255, 255, 0.08);
  padding: 28px;
  border-radius: 22px;
  font-size: 26px;
  text-align: center;
}

.squad-box {
background: #3b0c7281;
  border-radius: 26px;
  padding: 35px;
  box-shadow: 0 20px 45px rgba(0, 0, 0, 0.35);
}

.position-section {
  margin-bottom: 42px;
}

.position-section:last-child {
  margin-bottom: 0;
}

.position-section h2 {
  font-size: 34px;
  margin: 0 0 22px;
}

.players-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 22px;
  align-items: stretch;
}

.player-card {
  height: 112px;
  display: grid;
  grid-template-columns: 58px minmax(0, 1fr) 42px;
  grid-template-rows: 1fr auto;
  column-gap: 16px;
  row-gap: 6px;
  align-items: center;

  background: #65108c;
  border-radius: 24px;
  padding: 16px;
  position: relative;
  overflow: hidden;

  transition: transform 0.2s ease, box-shadow 0.2s ease;
}


.player-photo {
  width: 58px;
  height: 58px;
  border-radius: 50%;
  object-fit: cover;
  background: white;
  flex-shrink: 0;
}

.player-photo,
.player-avatar {
  grid-column: 1;
  grid-row: 1 / 3;
}

.modal-photo {
  width: 82px;
  height: 82px;
  border-radius: 50%;
  object-fit: cover;
  background: white;
  flex-shrink: 0;
}

.player-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 18px 34px rgba(0, 0, 0, 0.45);
}

.player-flag {
  grid-column: 3;
  grid-row: 1;
  justify-self: end;
  align-self: center;

  width: 34px;
  height: 24px;
  object-fit: cover;
  border-radius: 4px;
}

.player-avatar {
  width: 58px;
  height: 58px;
  border-radius: 50%;
  background: white;
  color: #000000;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 26px;
  flex-shrink: 0;
}

.player-info {
  grid-column: 2;
  grid-row: 1 / 3;
  min-width: 0;
  align-self: center;
}

.player-info h3 {
  font-size: 18px;
  line-height: 1.05;
  margin: 0 0 5px;
  max-height: 46px;
  overflow: hidden;
}

.player-info p {
  font-size: 19px;
  line-height: 1;
  margin: 0 0 4px;
}

.player-info span {
  display: inline-block;
  font-size: 15px;
  line-height: 1;
  font-weight: 800;
  opacity: 0.95;
}

.more-btn {
  grid-column: 3;
  grid-row: 2;
  justify-self: end;
  align-self: end;

  border: none;
  background: rgba(255, 255, 255, 0.24);
  color: white;
  padding: 6px 13px;
  border-radius: 999px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 13px;
  font-weight: 800;
  cursor: pointer;
  white-space: nowrap;
}

.more-btn:hover {
  background: rgba(255, 255, 255, 0.35);
  transform: translateY(-2px);
}

.player-card {
  position: relative;
}

.player-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(20, 0, 30, 0.72);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  padding: clamp(12px, 3vw, 30px);
}

.player-modal {
  width: min(680px, 92vw);
  max-height: 88vh;
  overflow-y: auto;

  background: linear-gradient(180deg, #74108d 0%, #3b0c72 100%);
  border-radius: clamp(18px, 3vw, 30px);
  padding: clamp(20px, 3vw, 30px);
  color: white;
  position: relative;
  box-shadow: 0 30px 80px rgba(0, 0, 0, 0.5);
}

.modal-close {
  position: absolute;
  top: 18px;
  right: 22px;
  border: none;
  background: rgba(255, 255, 255, 0.18);
  color: white;
  width: 38px;
  height: 38px;
  border-radius: 50%;
  font-size: 26px;
  cursor: pointer;
}

.modal-header {
  display: flex;
  align-items: center;
  gap: clamp(14px, 2vw, 22px);
  margin-bottom: 22px;
}

.modal-avatar {
  width: 82px;
  height: 82px;
  border-radius: 50%;
  background: white;
  color: #4a0c74;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 34px;
  flex-shrink: 0;
}

.modal-header h2 {
  font-size: clamp(26px, 4vw, 36px);
  margin: 0 0 6px;
}

.modal-header p {
  font-size: clamp(16px, 2vw, 20px);
  margin: 0;
  opacity: 0.9;
}

.player-details-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 14px;
}

.detail-box,
.notes-box {
  background: rgba(255, 255, 255, 0.12);
  border-radius: 16px;
  padding: 14px 16px;
}

.detail-box span,
.notes-box span {
  display: block;
  font-size: 14px;
  opacity: 0.75;
  margin-bottom: 5px;
}

.detail-box strong {
  font-size: clamp(18px, 2.5vw, 23px);
}

.notes-box {
  margin-top: 14px;
}

.notes-box p {
  margin: 0;
  font-size: 20px;
}

.live-stats-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
  margin-top: 14px;
}

.notes-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.notes-textarea {
  width: 100%;
  min-height: 56px;
  margin-top: 10px;
  resize: vertical;
  border: none;
  outline: none;
  border-radius: 14px;
  padding: 12px 14px;
  box-sizing: border-box;
  font-family: 'PremierLeague', sans-serif;
  font-size: 16px;
  font-weight: 700;
  color: white;
  background: rgba(255, 255, 255, 0.14);
}

.notes-textarea::placeholder {
  color: rgba(255, 255, 255, 0.65);
}

.save-notes-btn {
  margin-top: 14px;
  border: none;
  background: white;
  color: #4b006e;
  padding: 10px 20px;
  border-radius: 999px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 16px;
  font-weight: 900;
  cursor: pointer;
}

.save-notes-btn:hover {
  transform: translateY(-2px);
}

.save-notes-btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.save-success {
  background: #20c05c;
  color: white;
  font-size: 15px;
  font-weight: 800;
  padding: 10px 16px;
  border-radius: 12px;
  width: fit-content;

  animation: fadeInOut 2s ease;
}
.notes-actions {
  margin-top: 14px;
  display: flex;
  align-items: center;
  gap: 14px;
}

.custom-status-dropdown {
  position: relative;
}

.selected-status {
  height: 46px;
  padding: 0 18px;
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.14);

  display: flex;
  align-items: center;
  justify-content: space-between;

  font-size: 1.05rem;
  font-weight: 900;
  cursor: pointer;
}

.status-option {
  padding: 14px 18px;
  font-size: 1rem;
  font-weight: 900;
  cursor: pointer;
}

.selected-status:hover {
  background: rgba(255,255,255,0.2);
}

.dropdown-arrow {
  font-size: 14px;
  transition: transform 0.2s ease;
}

.dropdown-arrow.open {
  transform: rotate(180deg);
}

.status-options {
  position: absolute;
  top: calc(100% + 10px);
  left: 0;
  right: 0;

  background: #5f1380;

  border-radius: 18px;

  overflow: hidden;

  box-shadow: 0 18px 40px rgba(0,0,0,0.35);

  z-index: 100;
}

.status-option {
  padding: 15px 18px;

  font-size: 15px;
  font-weight: 800;

  cursor: pointer;

  transition: background 0.18s ease;
}

.status-option:hover {
  background: rgba(255,255,255,0.12);
}

.status-option.active {
  background: rgba(255,255,255,0.18);
}

.healthy-option, 
.selected-healthy {
  color: #63ff97;
}

.injured-option,
.selected-injured {
  color: #ff7a7a;
}

.excluded-option,
.selected-excluded {
  color: #ff3b3b;
}

.modal-photo,
.modal-avatar {
  width: clamp(58px, 8vw, 76px);
  height: clamp(58px, 8vw, 76px);
}

.player-title {
  display: flex;
  align-items: center;
  gap: 14px;
}

.player-title h2 {
  margin: 0;
}

.title-flag {
  width: 38px;
  height: 26px;
  object-fit: cover;
  border-radius: 6px;
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.35);
}

@keyframes fadeInOut {
  0% {
    opacity: 0;
    transform: translateY(6px);
  }

  15% {
    opacity: 1;
    transform: translateY(0);
  }

  85% {
    opacity: 1;
  }

  100% {
    opacity: 0;
  }
}

@media (max-width: 1100px) {
  .players-grid {
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (max-width: 800px) {
  .players-page {
    padding: 105px 20px 40px;
  }

  .players-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .section-header h1 {
    font-size: 38px;
  }
}

@media (max-width: 700px) {
  .player-modal {
    width: 94vw;
    max-height: 86vh;
  }

  .player-details-grid,
  .live-stats-grid {
    grid-template-columns: 1fr;
  }

  .modal-header {
    align-items: flex-start;
  }

  .modal-flag {
    width: 40px;
    height: 28px;
  }
}

@media (max-width: 520px) {
  .players-grid {
    grid-template-columns: 1fr;
  }
}
</style>