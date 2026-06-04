<template>
  <main class="formations-page">
    <section class="formations-container">
      <div class="formations-hero">
        <div>
          <h1>Formacje</h1>
          <div class="header-line"></div>
          <p>Planuj ustawienie zespołu i treningi pod najbliższe mecze.</p>
        </div>

        <div class="tab-switcher">
          <button
            :class="{ active: activeTab === 'formations' }"
            @click="activeTab = 'formations'"
          >
            Formacje
          </button>

          <button
            :class="{ active: activeTab === 'trainings' }"
            @click="activeTab = 'trainings'"
          >
            Treningi
          </button>
        </div>
      </div>

      <transition name="tab-fade" mode="out-in">
        <section
          v-if="activeTab === 'formations'"
          key="formations"
          class="formations-layout"
        >
          <div class="section-block">
            <h2>Wybór ustawienia :</h2>

            <div class="content-box formation-picker-box">
              <button
                v-for="formation in formations"
                :key="formation"
                class="formation-button"
                :class="{ active: selectedFormation === formation }"
                @click="selectFormation(formation)"
              >
                {{ formation }}
              </button>
              <button class="save-formation-button" @click="saveFormation">
  Zapisz formację
</button>

<p v-if="formationMessage" class="formation-message">
  {{ formationMessage }}
</p>
            </div>
          </div>

          <div class="section-block">
            <h2>Boisko :</h2>

            <div class="content-box pitch-box">
              <div class="pitch">
                <div
  v-for="position in currentPositions"
  :key="position.id"
  class="player-slot"
  :class="{ filled: selectedPlayers[position.id] }"
  :style="{ top: position.top, left: position.left }"
  @click="openPlayerModal(position)"
>
  <template v-if="selectedPlayers[position.id]">
    <button
  class="remove-player-btn"
  @click.stop="removePlayer(position.id)"
>
  ×
</button>
  <img
    v-if="selectedPlayers[position.id].photo"
    :src="selectedPlayers[position.id].photo"
    alt=""
    class="slot-player-photo"
  />

  <div v-else class="slot-avatar">
    {{ getInitials(selectedPlayers[position.id].name) }}
  </div>

  <small>{{ position.label }}</small>
<div class="slot-number">
  #{{ selectedPlayers[position.id].number || '?' }}
</div>
</template>

<template v-else>
  <span>{{ position.label }}</span>
  <strong>+</strong>
</template>
</div>
              </div>
            </div>
          </div>
        </section>

        <section
          v-else
          key="trainings"
          class="trainings-layout"
        >
          <div class="section-block">
            <h2>Nowy trening :</h2>

            <div class="content-box training-form-box">
              <div class="form-grid">
                <input v-model="training.title" placeholder="Nazwa treningu" />
                <input v-model="training.date" type="date" />

                <select v-model="training.type">
                  <option value="">Typ treningu</option>
                  <option>Taktyczny</option>
                  <option>Siłowy</option>
                  <option>Regeneracyjny</option>
                  <option>Strzelecki</option>
                  <option>Defensywny</option>
                </select>

                <select v-model="training.intensity">
                  <option value="">Intensywność</option>
                  <option>Niska</option>
                  <option>Średnia</option>
                  <option>Wysoka</option>
                </select>
              </div>

              <textarea
                v-model="training.description"
                placeholder="Opis treningu..."
              ></textarea>

              <button class="save-button" @click="saveTraining">
  {{ editingTrainingId ? 'Zapisz zmiany' : 'Zapisz trening' }}
</button>

<button
  v-if="editingTrainingId"
  class="cancel-edit-button"
  @click="resetTrainingForm"
>
  Anuluj edycję
</button>
            </div>
          </div>

          <div class="section-block">
            <h2>Zaplanowane treningi :</h2>

            <div class="content-box trainings-list-box">
  <div v-if="trainingsLoading" class="empty-state">
    Ładowanie treningów...
  </div>

  <div v-else-if="trainings.length === 0" class="empty-state">
    Brak zaplanowanych treningów.
  </div>

  <div v-else class="training-cards">
    <div
      v-for="item in trainings"
      :key="item.id"
      class="training-card"
    >
      <div>
        <h3>{{ item.name }}</h3>
        <p class="training-date">{{ formatTrainingDate(item.date) }}</p>
        <p class="training-meta">
          {{ item.type }} • {{ item.intensity }}
        </p>
        <p class="training-description">
          {{ item.description || 'Brak opisu.' }}
        </p>
      </div>

      <div class="training-actions">
        <button @click="editTraining(item)">Edytuj</button>
        <button class="delete" @click="deleteTraining(item.id)">Usuń</button>
      </div>
    </div>
  </div>
</div>
          </div>
        </section>
      </transition>
      <div
  v-if="showPlayerModal"
  class="modal-overlay"
  @click.self="closePlayerModal"
>
  <div class="players-modal">
    <button class="modal-close" @click="closePlayerModal">×</button>

    <h2>Wybierz zawodnika</h2>
    <p class="modal-subtitle">
      Pozycja: {{ selectedPosition?.label }}
    </p>

    <div v-if="playersLoading" class="modal-info">
      Ładowanie zawodników...
    </div>

    <div v-else-if="availablePlayers.length === 0" class="modal-info">
  Brak dostępnych zawodników dla tej pozycji.
</div>

<div v-else class="players-select-list">

  <button
    v-for="player in recommendedPlayers"
    :key="player.externalPlayerId || player.id || player.name"
    class="player-option"
    @click="selectPlayer(player)"
  >
    <img
      v-if="player.photo"
      :src="player.photo"
      alt=""
    />

    <div class="option-avatar" v-else>
      {{ getInitials(player.name) }}
    </div>

    <div>
      <strong>{{ player.name }}</strong>
      <span>
        {{ translatePosition(player.position) }}
        · #{{ player.number || '?' }}
      </span>
    </div>
  </button>

  <div
    v-if="showAllPlayers && otherPlayers.length > 0"
    class="players-divider"
  >
    <span>Pozostali zawodnicy</span>
  </div>

  <button
    v-if="showAllPlayers"
    v-for="player in otherPlayers"
    :key="'other-' + (player.externalPlayerId || player.id || player.name)"
    class="player-option secondary-option"
    @click="selectPlayer(player)"
  >
    <img
      v-if="player.photo"
      :src="player.photo"
      alt=""
    />

    <div class="option-avatar" v-else>
      {{ getInitials(player.name) }}
    </div>

    <div>
      <strong>{{ player.name }}</strong>
      <span>
        {{ translatePosition(player.position) }}
        · #{{ player.number || '?' }}
      </span>
    </div>
  </button>

</div>
    <button
 v-if="
  !showAllPlayers &&
  otherPlayers.length > 0 &&
  selectedPosition?.label !== 'GK'
"
  class="more-button"
  @click="showAllPlayers = true"
>
  Pokaż więcej zawodników
</button>
  </div>
</div>
    </section>
  </main>
<transition name="toast">
  <div
    v-if="toast.show"
    class="toast"
    :class="toast.type"
  >
    {{ toast.message }}
  </div>
</transition>
</template>

<script>
import api from '../api/axios'
export default {
  name: 'FormationsView',

  data() {
    return {
      activeTab: 'formations',
      selectedFormation: '3-4-3',
      players: [],
playersLoading: true,

showPlayerModal: false,
selectedPosition: null,
selectedPlayers: {},

      formations: [
  '3-4-3',
  '3-5-2',
  '4-1-4-1', 
  '4-2-3-1',
  '4-3-3',
  '4-4-2',
  '4-5-1',
  '5-3-2',

],
trainings: [],
trainingsLoading: false,
editingTrainingId: null,

      training: {
        title: '',
        date: '',
        type: '',
        intensity: '',
        description: ''
      },
      showAllPlayers: false,
      formationMessage: '',
  toast: {
  show: false,
  message: '',
  type: 'success'
}
    }
},

  computed: {
    currentPositions() {
      if (this.selectedFormation === '4-2-3-1') {
        return [
  { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
  { id: 2, label: 'LB', role: 'Defender', top: '66%', left: '18%' },
  { id: 3, label: 'CB', role: 'Defender', top: '68%', left: '38%' },
  { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '62%' },
  { id: 5, label: 'RB', role: 'Defender', top: '66%', left: '82%' },
  { id: 6, label: 'CDM', role: 'Midfielder', top: '50%', left: '35%' },
{ id: 7, label: 'CDM', role: 'Midfielder', top: '50%', left: '65%' },
{ id: 8, label: 'LW', role: 'Attacker', top: '29%', left: '20%' },
{ id: 9, label: 'CAM', role: 'Midfielder', top: '31%', left: '50%' },
{ id: 10, label: 'RW', role: 'Attacker', top: '29%', left: '80%' },
{ id: 11, label: 'ST', role: 'Attacker', top: '12%', left: '50%' }
]
      }
if (this.selectedFormation === '3-5-2') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
    { id: 2, label: 'CB', role: 'Defender', top: '68%', left: '30%' },
    { id: 3, label: 'CB', role: 'Defender', top: '70%', left: '50%' },
    { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '70%' },
    { id: 5, label: 'LM', role: 'Midfielder', top: '48%', left: '15%' },
    { id: 6, label: 'CM', role: 'Midfielder', top: '48%', left: '35%' },
    { id: 7, label: 'CM', role: 'Midfielder', top: '48%', left: '50%' },
    { id: 8, label: 'CM', role: 'Midfielder', top: '48%', left: '65%' },
    { id: 9, label: 'RM', role: 'Midfielder', top: '48%', left: '85%' },
    { id: 10, label: 'ST', role: 'Attacker', top: '16%', left: '40%' },
    { id: 11, label: 'ST', role: 'Attacker', top: '16%', left: '60%' }
  ]
}

if (this.selectedFormation === '3-4-3') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
    { id: 2, label: 'CB', role: 'Defender', top: '68%', left: '30%' },
    { id: 3, label: 'CB', role: 'Defender', top: '70%', left: '50%' },
    { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '70%' },
    { id: 5, label: 'LM', role: 'Midfielder', top: '48%', left: '20%' },
    { id: 6, label: 'CM', role: 'Midfielder', top: '48%', left: '40%' },
    { id: 7, label: 'CM', role: 'Midfielder', top: '48%', left: '60%' },
    { id: 8, label: 'RM', role: 'Midfielder', top: '48%', left: '80%' },
    { id: 9, label: 'LW', role: 'Attacker', top: '20%', left: '25%' },
    { id: 10, label: 'ST', role: 'Attacker', top: '12%', left: '50%' },
    { id: 11, label: 'RW', role: 'Attacker', top: '20%', left: '75%' }
  ]
}

if (this.selectedFormation === '5-3-2') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
    { id: 2, label: 'LWB', role: 'Defender', top: '62%', left: '12%' },
    { id: 3, label: 'CB', role: 'Defender', top: '68%', left: '32%' },
    { id: 4, label: 'CB', role: 'Defender', top: '70%', left: '50%' },
    { id: 5, label: 'CB', role: 'Defender', top: '68%', left: '68%' },
    { id: 6, label: 'RWB', role: 'Defender', top: '62%', left: '88%' },
    { id: 7, label: 'CM', role: 'Midfielder', top: '43%', left: '32%' },
    { id: 8, label: 'CM', role: 'Midfielder', top: '45%', left: '50%' },
    { id: 9, label: 'CM', role: 'Midfielder', top: '43%', left: '68%' },
    { id: 10, label: 'ST', role: 'Attacker', top: '16%', left: '40%' },
    { id: 11, label: 'ST', role: 'Attacker', top: '16%', left: '60%' }
  ]
}

if (this.selectedFormation === '4-1-4-1') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
    { id: 2, label: 'LB', role: 'Defender', top: '66%', left: '18%' },
    { id: 3, label: 'CB', role: 'Defender', top: '68%', left: '38%' },
    { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '62%' },
    { id: 5, label: 'RB', role: 'Defender', top: '66%', left: '82%' },
    { id: 6, label: 'CDM', role: 'Midfielder', top: '52%', left: '50%' },
    { id: 7, label: 'LM', role: 'Midfielder', top: '34%', left: '18%' },
    { id: 8, label: 'CM', role: 'Midfielder', top: '34%', left: '40%' },
    { id: 9, label: 'CM', role: 'Midfielder', top: '34%', left: '60%' },
    { id: 10, label: 'RM', role: 'Midfielder', top: '34%', left: '82%' },
    { id: 11, label: 'ST', role: 'Attacker', top: '12%', left: '50%' }
  ]
}

if (this.selectedFormation === '4-5-1') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
    { id: 2, label: 'LB', role: 'Defender', top: '66%', left: '18%' },
    { id: 3, label: 'CB', role: 'Defender', top: '68%', left: '38%' },
    { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '62%' },
    { id: 5, label: 'RB', role: 'Defender', top: '66%', left: '82%' },
    { id: 6, label: 'LM', role: 'Midfielder', top: '44%', left: '16%' },
    { id: 7, label: 'CM', role: 'Midfielder', top: '45%', left: '35%' },
    { id: 8, label: 'CM', role: 'Midfielder', top: '46%', left: '50%' },
    { id: 9, label: 'CM', role: 'Midfielder', top: '45%', left: '65%' },
    { id: 10, label: 'RM', role: 'Midfielder', top: '44%', left: '84%' },
    { id: 11, label: 'ST', role: 'Attacker', top: '14%', left: '50%' }
  ]
}
if (this.selectedFormation === '4-3-3') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },
    { id: 2, label: 'LB', role: 'Defender', top: '66%', left: '18%' },
    { id: 3, label: 'CB', role: 'Defender', top: '68%', left: '38%' },
    { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '62%' },
    { id: 5, label: 'RB', role: 'Defender', top: '66%', left: '82%' },
    { id: 6, label: 'CM', role: 'Midfielder', top: '45%', left: '32%' },
    { id: 7, label: 'CM', role: 'Midfielder', top: '50%', left: '50%' },
    { id: 8, label: 'CM', role: 'Midfielder', top: '45%', left: '68%' },
    { id: 9, label: 'LW', role: 'Attacker', top: '25%', left: '22%' },
    { id: 10, label: 'ST', role: 'Attacker', top: '14%', left: '50%' },
    { id: 11, label: 'RW', role: 'Attacker', top: '25%', left: '78%' }
  ]
}
if (this.selectedFormation === '4-4-2') {
  return [
    { id: 1, label: 'GK', role: 'Goalkeeper', top: '90%', left: '50%' },

    { id: 2, label: 'LB', role: 'Defender', top: '66%', left: '18%' },
    { id: 3, label: 'CB', role: 'Defender', top: '68%', left: '38%' },
    { id: 4, label: 'CB', role: 'Defender', top: '68%', left: '62%' },
    { id: 5, label: 'RB', role: 'Defender', top: '66%', left: '82%' },

    { id: 6, label: 'LM', role: 'Midfielder', top: '43%', left: '18%' },
    { id: 7, label: 'CM', role: 'Midfielder', top: '45%', left: '40%' },
    { id: 8, label: 'CM', role: 'Midfielder', top: '45%', left: '60%' },
    { id: 9, label: 'RM', role: 'Midfielder', top: '43%', left: '82%' },

    { id: 10, label: 'ST', role: 'Attacker', top: '16%', left: '40%' },
    { id: 11, label: 'ST', role: 'Attacker', top: '16%', left: '60%' }
  ]
}
    },
    recommendedPlayers() {
  if (!this.selectedPosition) return []

  const selectedIds = Object.values(this.selectedPlayers)
    .map(player => player.externalPlayerId || player.id || player.name)

  return this.players.filter(player => {
    const id = player.externalPlayerId || player.id || player.name

    return (
      !selectedIds.includes(id) &&
      this.isPlayerCompatibleWithSlot(player, this.selectedPosition)
    )
  })
},

otherPlayers() {
  if (!this.selectedPosition) return []

  const selectedIds = Object.values(this.selectedPlayers)
    .map(player => player.externalPlayerId || player.id || player.name)

  return this.players.filter(player => {
    const id = player.externalPlayerId || player.id || player.name

    return (
      !selectedIds.includes(id) &&
      player.position !== 'Goalkeeper' &&
      !this.isPlayerCompatibleWithSlot(player, this.selectedPosition)
    )
  })
},

availablePlayers() {
  return this.showAllPlayers
    ? [...this.recommendedPlayers, ...this.otherPlayers]
    : this.recommendedPlayers
}
  },

  async mounted() {
  await this.fetchPlayers()
  await this.loadCurrentFormation()
  await this.loadTrainings()
},

methods: {
  async fetchPlayers() {
    try {
      this.playersLoading = true

      const response = await api.get('/Players/my-team-live')
      this.players = response.data.$values || response.data || []
    } catch (error) {
      console.error(error)
    } finally {
      this.playersLoading = false
    }
  },

  async selectFormation(formation) {
  if (this.selectedFormation === formation) return

  this.selectedFormation = formation
  this.selectedPlayers = {}

  await this.loadCurrentFormation()
},

async saveFormation() {
  try {
    const playersPayload = this.currentPositions.map(position => {
      const player = this.selectedPlayers[position.id]

      return {
        slotId: position.id,
        positionLabel: position.label,
        externalPlayerId: player?.externalPlayerId || null,
        playerName: player?.name || null,
        playerPhoto: player?.photo || null,
        playerNumber: player?.number || null
      }
    })

    await api.put(`/Formations/current/${this.selectedFormation}`, {
      name: `Formacja ${this.selectedFormation}`,
      formation: this.selectedFormation,
      players: playersPayload
    })

    this.formationMessage = 'Formacja została zapisana.'
  } catch (error) {
    console.error(error)
    this.formationMessage = 'Nie udało się zapisać formacji.'
  }

  setTimeout(() => {
    this.formationMessage = ''
  }, 3000)
},

async loadCurrentFormation() {
  try {
    const response = await api.get(`/Formations/current/${this.selectedFormation}`)
    const formation = response.data

    const loadedPlayers = {}

    formation.players.forEach(slot => {
      if (!slot.externalPlayerId) return

      loadedPlayers[slot.slotId] = {
        externalPlayerId: slot.externalPlayerId,
        name: slot.playerName,
        photo: slot.playerPhoto,
        number: slot.playerNumber
      }
    })

    this.selectedPlayers = loadedPlayers
  } catch (error) {
    console.error(error)
  }
},

  removePlayer(positionId) {
  const updated = { ...this.selectedPlayers }
  delete updated[positionId]
  this.selectedPlayers = updated
},

  isPlayerCompatibleWithSlot(player, slot) {
  if (!player || !slot) return false

  const playerPosition = player.position
  const slotLabel = slot.label

  if (slotLabel === 'GK') {
    return playerPosition === 'Goalkeeper'
  }

  if (['LB', 'CB', 'RB', 'LWB', 'RWB'].includes(slotLabel)) {
    return playerPosition === 'Defender'
  }

  if (['CDM', 'CM', 'CAM'].includes(slotLabel)) {
    return playerPosition === 'Midfielder'
  }

  if (['LW', 'ST', 'RW'].includes(slotLabel)) {
    return playerPosition === 'Attacker'
  }

  return playerPosition === slot.role
},

  openPlayerModal(position) {
  this.selectedPosition = position
  this.showPlayerModal = true
  this.showAllPlayers = false
},

  closePlayerModal() {
    this.showPlayerModal = false
    this.selectedPosition = null
  },

  selectPlayer(player) {
    if (!this.selectedPosition) return

    this.selectedPlayers = {
      ...this.selectedPlayers,
      [this.selectedPosition.id]: player
    }

    this.closePlayerModal()
  },

  getInitials(name) {
    if (!name) return '?'

    return name
      .split(' ')
      .map(part => part[0])
      .join('')
      .slice(0, 2)
      .toUpperCase()
  },

  translatePosition(position) {
    const map = {
      Goalkeeper: 'Bramkarz',
      Defender: 'Obrońca',
      Midfielder: 'Pomocnik',
      Attacker: 'Napastnik'
    }

    return map[position] || position || '-'
  },
  async loadTrainings() {
  try {
    this.trainingsLoading = true

    const response = await api.get('/TrainingPlans/my-team')
    this.trainings = response.data.$values || response.data || []
  } catch (error) {
    console.error('Błąd ładowania treningów:', error)
  } finally {
    this.trainingsLoading = false
  }
},

async saveTraining() {
if (!this.validateTrainingForm()) return

  try {
    const payload = {
      name: this.training.title,
      date: this.training.date,
      type: this.training.type,
      intensity: this.training.intensity,
      description: this.training.description,
      exercises: []
    }

    if (this.editingTrainingId) {
      await api.put(`/TrainingPlans/${this.editingTrainingId}`, payload)
    } else {
      await api.post('/TrainingPlans', payload)
    }

    await this.loadTrainings()
    this.showToast(
  this.editingTrainingId ? 'Trening został zaktualizowany.' : 'Trening został zapisany.',
  'success'
)
    this.resetTrainingForm()
  } catch (error) {
    console.error('Błąd zapisu treningu:', error)
  }
},

editTraining(item) {
  this.editingTrainingId = item.id

  this.training = {
    title: item.name || '',
    date: item.date ? item.date.substring(0, 10) : '',
    type: item.type || '',
    intensity: item.intensity || '',
    description: item.description || ''
  }
},

async deleteTraining(id) {
  try {
    await api.delete(`/TrainingPlans/${id}`)
    await this.loadTrainings()

    if (this.editingTrainingId === id) {
      this.resetTrainingForm()
    }
  } catch (error) {
    console.error('Błąd usuwania treningu:', error)
  }
},

resetTrainingForm() {
  this.training = {
    title: '',
    date: '',
    type: '',
    intensity: '',
    description: ''
  }

  this.editingTrainingId = null
},

formatTrainingDate(date) {
  if (!date) return '-'

  return new Date(date).toLocaleDateString('pl-PL')
},
showToast(message, type = 'success') {
  this.toast.message = message
  this.toast.type = type
  this.toast.show = true

  setTimeout(() => {
    this.toast.show = false
  }, 3000)
},

validateTrainingForm() {
  if (!this.training.title.trim()) {
    this.showToast('Podaj nazwę treningu.', 'error')
    return false
  }

  if (!this.training.date) {
    this.showToast('Wybierz datę treningu.', 'error')
    return false
  }

  const selectedDate = new Date(this.training.date)
  const today = new Date()

  selectedDate.setHours(0, 0, 0, 0)
  today.setHours(0, 0, 0, 0)

  if (selectedDate < today) {
    this.showToast('Data treningu musi być w przyszłości.', 'error')
    return false
  }

  if (!this.training.type) {
    this.showToast('Wybierz typ treningu.', 'error')
    return false
  }

  if (!this.training.intensity) {
    this.showToast('Wybierz intensywność treningu.', 'error')
    return false
  }

  return true
},
}
}
</script>

<style scoped>
.formations-page {
  min-height: 100vh;
  padding: 40px 40px 70px;
  color: white;
  font-family: 'PremierLeague', sans-serif;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
}

.formations-container {
  max-width: 1450px;
  margin: 0 auto;
}

.formations-hero {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(59, 12, 114, 0.55);
  border-radius: 32px;
  padding: 35px 42px;
  margin-bottom: 34px;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.22);
}

.formations-hero h1 {
  font-size: 4.4rem;
  margin: 0;
  font-weight: 800;
}

.header-line {
  width: 420px;
  height: 5px;
  background: white;
  border-radius: 999px;
  margin: 10px 0 12px;
}

.formations-hero p {
  font-size: 1.25rem;
  margin: 0;
  opacity: 0.9;
}

.tab-switcher {
  display: flex;
  gap: 10px;
  padding: 8px;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.12);
}

.tab-switcher button {
  border: none;
  border-radius: 999px;
  padding: 13px 30px;
  background: transparent;
  color: white;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1rem;
  font-weight: 900;
  cursor: pointer;
  transition: 0.2s ease;
}

.tab-switcher button.active {
  background: #d71920;
  box-shadow: 0 8px 22px rgba(215, 25, 32, 0.35);
}

.formations-layout,
.trainings-layout {
  display: grid;
  grid-template-columns: 34% 66%;
  gap: 34px;
}

.section-block h2 {
  margin: 0 0 18px;
  color: white;
  font-size: 2.15rem;
  font-weight: 800;
}

.content-box {
  background: rgba(59, 12, 114, 0.58);
  border-radius: 30px;
  padding: 30px;
  box-shadow: 0 14px 34px rgba(0, 0, 0, 0.18);
}

.formation-picker-box {
  display: grid;
  gap: 16px;
}

.formation-button {
  border: none;
  background: rgba(255, 255, 255, 0.12);
  color: white;
  padding: 20px;
  border-radius: 22px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1.5rem;
  font-weight: 900;
  cursor: pointer;
  transition: 0.2s ease;
}

.formation-button:hover,
.formation-button.active {
  background: #d71920;
  transform: translateY(-2px);
}

.pitch-box {
  padding: 26px;
}

.pitch {
  position: relative;
  height: 650px;
  border-radius: 30px;
  overflow: hidden;
  background:
    linear-gradient(rgba(255,255,255,0.09) 2px, transparent 2px),
    linear-gradient(90deg, rgba(255,255,255,0.09) 2px, transparent 2px),
    linear-gradient(180deg, rgba(25, 132, 68, 0.95), rgba(10, 92, 52, 0.95));
  background-size: 100% 50%, 50% 100%, 100% 100%;
  border: 4px solid rgba(255, 255, 255, 0.75);
}

.player-slot {
  position: absolute;
  transform: translate(-50%, -50%);
  width: 86px;
  height: 86px;
  border-radius: 50%;
  background: #d71920;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 2px;
  box-shadow: 0 12px 24px rgba(0,0,0,0.28);
}

.player-slot span {
  font-size: 1rem;
  font-weight: 900;
}

.player-slot strong {
  font-size: 1.6rem;
  line-height: 1;
}

.training-form-box input,
.training-form-box select,
.training-form-box textarea {
  width: 100%;
  box-sizing: border-box;
  border: none;
  outline: none;
  border-radius: 18px;
  padding: 15px 18px;
  background: rgba(255, 255, 255, 0.12);
  color: white;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1rem;
  font-weight: 700;
}

.training-form-box input::placeholder,
.training-form-box textarea::placeholder {
  color: rgba(255, 255, 255, 0.65);
}

.training-form-box select option {
  color: #32003f;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 18px;
  margin-bottom: 18px;
}

.training-form-box textarea {
  min-height: 210px;
  resize: vertical;
}

.save-button {
  margin-top: 18px;
  border: none;
  background: #d71920;
  color: white;
  padding: 14px 26px;
  border-radius: 18px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1rem;
  font-weight: 900;
  cursor: pointer;
}

.trainings-list-box {
  min-height: 430px;
}

.empty-state {
  height: 360px;
  display: grid;
  place-items: center;
  font-size: 1.4rem;
  font-weight: 800;
  opacity: 0.8;
}

.tab-fade-enter-active,
.tab-fade-leave-active {
  transition: all 0.22s ease;
}

.tab-fade-enter-from {
  opacity: 0;
  transform: translateY(12px);
}

.tab-fade-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

.player-slot {
  cursor: pointer;
  transition: 0.2s ease;
}

.player-slot:hover {
  transform: translate(-50%, -50%) scale(1.08);
}

.player-slot.filled {
  width: 96px;
  height: 112px;
  border-radius: 24px;
  background: rgba(255, 255, 255, 0.94);
  color: #32003f;
  padding: 8px 6px;
  gap: 4px;
}


.player-slot small {
  font-size: 1.5rem;
  font-weight: 900;
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

.players-modal {
  position: relative;
  width: min(720px, 95vw);
  max-height: 88vh;
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

.players-modal h2 {
  font-size: 2.4rem;
  margin: 0;
}

.modal-subtitle {
  opacity: 0.8;
  margin: 8px 0 24px;
  font-size: 1.1rem;
}

.modal-info {
  background: rgba(255, 255, 255, 0.12);
  padding: 24px;
  border-radius: 20px;
  font-size: 1.3rem;
  font-weight: 800;
}

.players-select-list {
  display: grid;
  gap: 12px;
}

.player-option {
  width: 100%;
  border: none;
  background: rgba(255, 255, 255, 0.1);
  color: white;
  border-radius: 20px;
  padding: 14px 16px;
  display: flex;
  align-items: center;
  gap: 14px;
  text-align: left;
  font-family: 'PremierLeague', sans-serif;
  cursor: pointer;
  transition: 0.2s ease;
}

.player-option:hover {
  background: rgba(215, 25, 32, 0.85);
  transform: translateY(-1px);
}

.player-option img,
.option-avatar {
  width: 52px;
  height: 52px;
  border-radius: 50%;
  background: white;
  color: #32003f;
  object-fit: cover;
  display: grid;
  place-items: center;
  font-size: 1.1rem;
  font-weight: 900;
}

.player-option strong {
  display: block;
  font-size: 1.1rem;
}

.player-option span {
  display: block;
  opacity: 0.75;
  margin-top: 4px;
}

.slot-player-photo,
.slot-avatar {
  width: 54px;
  height: 54px;
  border-radius: 50%;
  object-fit: cover;
  background: white;
  border: none;
}

.slot-avatar {
  width: 74px;
  height: 74px;
  border-radius: 50%;
  background: white;
  color: #32003f;
  display: grid;
  place-items: center;
  font-size: 1.1rem;
  font-weight: 900;
  border: 3px solid rgba(255,255,255,0.85);
}

.player-slot {
  position: absolute;
  overflow: visible;
}
.player-slot.filled small {
  margin-top: 2px;
  font-size: 0.82rem;
  font-weight: 900;
  color: #32003f;
}

.player-slot.filled .slot-number {
  font-size: 1.15rem;
  font-weight: 900;
  line-height: 1;
}

.player-slot small {
  margin-top: 2px;
  font-size: 0.9rem;
  font-weight: 900;
  color: #32003f;
}

.more-button {
  width: 100%;
  margin-top: 18px;
  border: none;
  background: rgba(255,255,255,0.12);
  color: white;
  padding: 16px;
  border-radius: 18px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1rem;
  font-weight: 900;
  cursor: pointer;
  transition: 0.2s ease;
}

.more-button:hover {
  background: rgba(255,255,255,0.2);
}

.players-divider {
  display: flex;
  align-items: center;
  gap: 14px;
  margin: 12px 0 4px;
  opacity: 0.75;
}

.players-divider::before,
.players-divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background: rgba(255,255,255,0.18);
}

.players-divider span {
  font-size: 0.9rem;
  font-weight: 800;
  letter-spacing: 0.04em;
  text-transform: uppercase;
}

.secondary-option {
  opacity: 0.82;
}

.remove-player-btn {
  position: absolute;
  top: -12px;
  right: -12px;
  width: 34px;
  height: 34px;
  border: 3px solid white;
  border-radius: 50%;
  background: #d71920;
  color: white;
  font-size: 24px;
  font-weight: 900;
  line-height: 1;
  cursor: pointer;
  display: grid;
  place-items: center;
  box-shadow: 0 8px 18px rgba(0,0,0,0.35);
  z-index: 20;
  padding: 0;
}

.remove-player-btn:hover {
  transform: scale(1.08);
}
.save-formation-button {
  margin-top: 10px;
  border: none;
  background: #ffffff;
  color: #32003f;
  padding: 18px;
  border-radius: 22px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1.2rem;
  font-weight: 900;
  cursor: pointer;
  transition: 0.2s ease;
}

.save-formation-button:hover {
  transform: translateY(-2px);
  opacity: 0.92;
}

.formation-message {
  margin: 6px 0 0;
  text-align: center;
  font-size: 1rem;
  font-weight: 900;
  color: white;
  opacity: 0.9;
}
.training-cards {
  display: grid;
  gap: 18px;
  max-height: 430px;
  overflow-y: auto;
  padding-right: 8px;
}

.training-card {
  background: rgba(255, 255, 255, 0.11);
  border-radius: 22px;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  gap: 20px;
}

.training-card h3 {
  margin: 0 0 8px;
  font-size: 1.45rem;
  font-weight: 900;
}

.training-date,
.training-meta,
.training-description {
  margin: 5px 0;
  opacity: 0.85;
  font-weight: 700;
}

.training-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.training-actions button,
.cancel-edit-button {
  border: none;
  background: white;
  color: #32003f;
  padding: 10px 16px;
  border-radius: 14px;
  font-family: 'PremierLeague', sans-serif;
  font-weight: 900;
  cursor: pointer;
}

.training-actions button.delete {
  background: #d71920;
  color: white;
}

.cancel-edit-button {
  margin-left: 12px;
  background: rgba(255, 255, 255, 0.18);
  color: white;
}
.toast {
  position: fixed;
  left: 50%;
  bottom: 150px;
  transform: translateX(-50%);
  min-width: 320px;
  max-width: 420px;
  padding: 18px 24px;
  border-radius: 18px;
  color: white;
  font-size: 1rem;
  font-weight: 800;
  z-index: 5000;
  box-shadow: 0 14px 35px rgba(0,0,0,0.25);
  backdrop-filter: blur(10px);
}

.toast.success {
  background: linear-gradient(135deg, rgba(46, 204, 113, 0.96), rgba(39, 174, 96, 0.96));
}

.toast.error {
  background: linear-gradient(135deg, rgba(231, 76, 60, 0.96), rgba(192, 57, 43, 0.96));
}

.toast-enter-active,
.toast-leave-active {
  transition: opacity 0.25s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
}

@media (max-width: 1100px) {
  .formations-hero {
    flex-direction: column;
    align-items: flex-start;
    gap: 24px;
  }

  .formations-layout,
  .trainings-layout {
    grid-template-columns: 1fr;
  }
}
</style>