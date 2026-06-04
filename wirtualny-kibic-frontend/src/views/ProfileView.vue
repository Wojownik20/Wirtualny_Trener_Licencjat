<template>
  <main class="profile-page">
    <div class="profile-container" v-if="profile">
      <section class="profile-grid">

        <!-- LEFT -->
        <div class="left-column">
          <div class="welcome-section">
            <h1>Profile</h1>
            <div class="welcome-line"></div>
          </div>

          <div class="section-block">
            <h2>Twój profil :</h2>

            <div class="content-box profile-main-box">
              <div class="avatar-wrap">
                <div class="avatar">
                  {{ initials }}
                </div>

                <img
                  v-if="profile.teamLogoUrl"
                  :src="profile.teamLogoUrl"
                  class="club-logo"
                  alt="Herb klubu"
                />
              </div>

              <h3>{{ profile.firstName }} {{ profile.lastName }}</h3>
              <p class="role">{{ formatRole(profile.role) }}</p>
              <p class="team">{{ profile.teamName }}</p>
            </div>
          </div>
        </div>

        <!-- RIGHT -->
        <div class="right-column">

          <div class="section-block">
            <h2>Dane konta :</h2>

            <div class="content-box details-box">
              <div class="detail-row">
                <span>Email</span>
                <strong>{{ profile.email }}</strong>
              </div>

              <div class="detail-row">
                <span>Rola</span>
                <strong>{{ formatRole(profile.role) }}</strong>
              </div>

              <div class="detail-row">
                <span>Drużyna</span>
                <strong>{{ profile.teamName }}</strong>
              </div>

              <div class="actions-row">
                <button @click="openEmailModal">Zmień email</button>
                <button @click="showPasswordModal = true">Zmień hasło</button>
              </div>
            </div>
          </div>

          <div class="section-block">
            <h2>Bezpieczeństwo :</h2>

            <div class="content-box details-box">
              <div class="detail-row">
                <span>Weryfikacja dwuetapowa</span>
                <strong :class="profile.twoFactorEnabled ? 'status-on' : 'status-off'">
                  {{ profile.twoFactorEnabled ? 'Włączona' : 'Wyłączona' }}
                </strong>
              </div>

              <div class="actions-row">
                <button
                  v-if="!profile.twoFactorEnabled"
                  @click="setup2FA"
                >
                  Ustaw 2FA
                </button>

                <button
                  v-if="profile.twoFactorEnabled"
                  class="danger-btn"
                  @click="disable2FA"
                >
                  Wyłącz 2FA
                </button>
              </div>

              <div v-if="twoFactorSetup" class="twofa-box">
                <p>Dodaj ten klucz do aplikacji Authenticator:</p>
                <img
  v-if="twoFactorSetup.authenticatorUri"
  class="qr-code"
  :src="getQrCodeUrl(twoFactorSetup.authenticatorUri)"
  alt="Kod QR 2FA"
/>
                <div class="secret-box">
                  {{ twoFactorSetup.sharedKey }}
                </div>

                <input
                  v-model="twoFactorCode"
                  placeholder="Kod z aplikacji"
                />

                <button @click="enable2FA">
                  Potwierdź 2FA
                </button>
              </div>
            </div>
          </div>

        </div>
      </section>
    </div>

    <div v-else class="loading-box">
      Ładowanie profilu...
    </div>

    <!-- EMAIL MODAL -->
    <div v-if="showEmailModal" class="modal-overlay">
      <div class="modal-box">
        <h2>Zmień email</h2>

        <input
          v-model="newEmail"
          placeholder="Nowy email"
        />

        <div class="modal-actions">
          <button @click="changeEmail">Zapisz</button>
          <button class="secondary-btn" @click="showEmailModal = false">
            Anuluj
          </button>
        </div>
      </div>
    </div>

    <!-- PASSWORD MODAL -->
    <div v-if="showPasswordModal" class="modal-overlay">
      <div class="modal-box">
        <h2>Zmień hasło</h2>

        <input
          v-model="currentPassword"
          type="password"
          placeholder="Aktualne hasło"
        />

        <input
          v-model="newPassword"
          type="password"
          placeholder="Nowe hasło"
        />
        <input
  v-model="confirmNewPassword"
  type="password"
  placeholder="Powtórz nowe hasło"
/>

        <div class="modal-actions">
          <button @click="changePassword">Zmień hasło</button>
          <button class="secondary-btn" @click="showPasswordModal = false">
            Anuluj
          </button>
        </div>
      </div>
    </div>
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
  data() {
    return {
      profile: null,

      showEmailModal: false,
      showPasswordModal: false,

      newEmail: '',

      currentPassword: '',
      newPassword: '',
        confirmNewPassword: '',
      twoFactorSetup: null,
      twoFactorCode: '',
      toast: {
  show: false,
  message: '',
  type: 'success'
},
    }
  },

  computed: {
    initials() {
      if (!this.profile) return '?'

      return (
        (this.profile.firstName?.[0] || '') +
        (this.profile.lastName?.[0] || '')
      ).toUpperCase()
    }
  },

  async mounted() {
    await this.fetchProfile()
  },

  methods: {
    async fetchProfile() {
      try {
        const res = await api.get('/Profile/me')
        this.profile = res.data
      } catch (err) {
        console.error(err)
      }
    },

    showToast(message, type = 'success') {
  this.toast.message = message
  this.toast.type = type
  this.toast.show = true

  setTimeout(() => {
    this.toast.show = false
  }, 3000)
},

    getQrCodeUrl(uri) {
  return `https://api.qrserver.com/v1/create-qr-code/?size=180x180&data=${encodeURIComponent(uri)}`
},

    formatRole(role) {
      if (!role) return 'Sztab'

      return role
        .replace(/([A-Z])/g, ' $1')
        .trim()
    },

    openEmailModal() {
      this.newEmail = this.profile.email
      this.showEmailModal = true
    },

    async changeEmail() {
      try {
        await api.put('/Profile/change-email', {
          newEmail: this.newEmail
        })

        this.showToast('Email został zmieniony.', 'success')

        this.showEmailModal = false
        this.newEmail = ''

        await this.fetchProfile()
      } catch (err) {
        console.error(err)
        this.showToast('Nie udało się zmienić emaila.', 'error')
      }
    },

    async changePassword() {
if (this.newPassword !== this.confirmNewPassword) {
  this.showToast('Nowe hasła nie są takie same.', 'error')
  return
}

      try {
        await api.put('/Profile/change-password', {
          currentPassword: this.currentPassword,
          newPassword: this.newPassword
        })

        this.confirmNewPassword = ''
        this.showToast('Hasło zostało zmienione.', 'success')

        this.showPasswordModal = false
        this.currentPassword = ''
        this.newPassword = ''
      } catch (err) {
        console.error(err)
        this.showToast('Nie udało się zmienić hasła.', 'error')
      }
    },

    async setup2FA() {
      try {
        const res = await api.get('/Profile/2fa/setup')
        this.twoFactorSetup = res.data
      } catch (err) {
        console.error(err)
      }
    },

    async enable2FA() {
      try {
        await api.post('/Profile/2fa/enable', {
          code: this.twoFactorCode
        })

        this.showToast('2FA zostało włączone.', 'success')

        this.twoFactorSetup = null
        this.twoFactorCode = ''

        await this.fetchProfile()
      } catch (err) {
        console.error(err)
        this.showToast('Nieprawidłowy kod 2FA.', 'error')
      }
    },

    async disable2FA() {
  const confirmed = confirm('Czy na pewno chcesz wyłączyć weryfikację dwuetapową?')

  if (!confirmed) return

  try {
    await api.post('/Profile/2fa/disable')

    this.showToast('2FA zostało wyłączone.', 'success')

    await this.fetchProfile()
  } catch (err) {
    console.error(err)
    this.showToast('Nie udało się wyłączyć 2FA.', 'error')
  }
}
  }
}
</script>

<style scoped>
.profile-page {
  min-height: calc(100vh - 72px);
  background: linear-gradient(180deg, #5b0b739c 0%, #32003f 100%);
  padding: 36px 42px;
  font-family: 'PremierLeague', sans-serif;
}

.profile-container {
  max-width: 1400px;
  margin: 0 auto;
}

.profile-grid {
  display: grid;
  grid-template-columns: 0.85fr 1.15fr;
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
  width: 300px;
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

.profile-main-box {
  min-height: 430px;
  padding: 38px 34px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

.avatar-wrap {
  position: relative;
  margin-bottom: 28px;
}

.avatar {
  width: 150px;
  height: 150px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.92);
  color: #32003f;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 4.2rem;
  font-weight: 900;
}

.club-logo {
  position: absolute;
  right: -24px;
  bottom: -12px;
  width: 72px;
  height: 72px;
  object-fit: contain;
  background: rgba(142, 25, 189, 0.74);
  border-radius: 50%;
  padding: 8px;
}

.profile-main-box h3 {
  margin: 0;
  font-size: 2.35rem;
  font-weight: 900;
  text-align: center;
}

.role {
  margin: 10px 0 0;
  font-size: 1.4rem;
  font-weight: 700;
  opacity: 0.85;
}

.team {
  margin: 16px 0 0;
  font-size: 1.6rem;
  font-weight: 900;
}

.details-box {
  padding: 30px;
  min-height: 210px;
}

.detail-row {
  display: grid;
  grid-template-columns: 220px 1fr;
  gap: 18px;
  align-items: center;
  padding: 14px 12px;
  border-radius: 18px;
  background: rgba(255, 255, 255, 0.07);
  margin-bottom: 14px;
}

.detail-row span {
  font-size: 1.05rem;
  font-weight: 700;
  opacity: 0.82;
}

.detail-row strong {
  font-size: 1.15rem;
  font-weight: 900;
  word-break: break-word;
}

.actions-row {
  display: flex;
  gap: 14px;
  margin-top: 22px;
  flex-wrap: wrap;
}

button {
  border: none;
  background: rgba(255, 255, 255, 0.92);
  color: #32003f;
  padding: 12px 22px;
  border-radius: 16px;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1rem;
  font-weight: 900;
  cursor: pointer;
  transition: 0.2s ease;
}

button:hover {
  transform: translateY(-2px);
  opacity: 0.92;
}

.danger-btn {
  background: rgba(255, 77, 77, 0.9);
  color: white;
}

.secondary-btn {
  background: rgba(255, 255, 255, 0.18);
  color: white;
}

.status-on {
  color: #4caf50;
}

.status-off {
  color: #ff4d4d;
}

.twofa-box {
  margin-top: 24px;
  padding: 22px;
  border-radius: 22px;
  background: rgba(255, 255, 255, 0.07);
}

.twofa-box p {
  margin: 0 0 14px;
  font-size: 1.05rem;
  font-weight: 800;
}

.secret-box {
  padding: 16px;
  border-radius: 16px;
  background: rgba(0, 0, 0, 0.22);
  font-family: monospace;
  font-size: 1rem;
  word-break: break-all;
  margin-bottom: 16px;
}

.twofa-box input,
.modal-box input {
  width: 100%;
  padding: 14px 16px;
  border: none;
  border-radius: 16px;
  margin-bottom: 14px;
  background: rgba(255, 255, 255, 0.12);
  color: white;
  font-family: 'PremierLeague', sans-serif;
  font-size: 1rem;
  box-sizing: border-box;
  outline: none;
}

.twofa-box input::placeholder,
.modal-box input::placeholder {
  color: rgba(255, 255, 255, 0.62);
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.68);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
  padding: 24px;
}

.modal-box {
  width: 460px;
  max-width: 100%;
  background: #3b0c72;
  color: white;
  border-radius: 28px;
  padding: 34px;
  box-shadow: 0 18px 45px rgba(0, 0, 0, 0.35);
}

.modal-box h2 {
  margin: 0 0 22px;
  font-size: 2.15rem;
  font-weight: 900;
}

.modal-actions {
  display: flex;
  gap: 14px;
  margin-top: 8px;
}

.loading-box {
  min-height: calc(100vh - 72px);
  background: linear-gradient(180deg, #5b0b739c 0%, #32003f 100%);
  color: white;
  font-family: 'PremierLeague', sans-serif;
  font-size: 2rem;
  font-weight: 900;
  display: flex;
  align-items: center;
  justify-content: center;
}

.qr-code {
  width: 180px;
  height: 180px;
  background: white;
  padding: 12px;
  border-radius: 18px;
  margin-bottom: 18px;
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
  background: linear-gradient(
    135deg,
    rgba(46, 204, 113, 0.96),
    rgba(39, 174, 96, 0.96)
  );
}

.toast.error {
  background: linear-gradient(
    135deg,
    rgba(231, 76, 60, 0.96),
    rgba(192, 57, 43, 0.96)
  );
}

.toast-enter-active,
.toast-leave-active {
  transition: opacity 0.25s ease;
}

.toast-enter-from,
.toast-leave-to {
  opacity: 0;
}

.toast-enter-to,
.toast-leave-from {
  opacity: 1;
}

@media (max-width: 1100px) {
  .profile-grid {
    grid-template-columns: 1fr;
    gap: 22px;
  }

  .welcome-section h1 {
    font-size: 3.2rem;
  }

  .section-block h2 {
    font-size: 1.8rem;
  }
}

@media (max-width: 768px) {
  .profile-page {
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

  .detail-row {
    grid-template-columns: 1fr;
    gap: 6px;
  }

  .profile-main-box h3 {
    font-size: 1.9rem;
  }

  .avatar {
    width: 125px;
    height: 125px;
    font-size: 3.5rem;
  }
}
</style>