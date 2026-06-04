<template>
  <div class="login-page">
    <div
      class="page-fade-overlay"
      :class="{ 'page-fade-overlay-show': animationStage >= 3 }"
    ></div>

    <div class="login-content">
      <div
        class="branding"
        :class="{
          'branding-move-down': animationStage >= 2,
          'branding-fade-out': animationStage >= 3
        }"
      >
        <div class="brand-row">
          <img :src="premierLogo" alt="Premier League" class="logo" />

          <div class="brand-text">
            <span class="divider"></span>
            <span class="app-text">Wirtualny Trener</span>
          </div>
        </div>
      </div>

      <transition name="form-switch" mode="out-in">
        <form
          v-if="currentStep === 'login'"
          key="login-form"
          class="login-form"
          :class="{ 'form-hide': animationStage >= 1 }"
          @submit.prevent="login"
        >
          <div class="form-group">
            <label for="email">E-mail :</label>
            <input
              id="email"
              v-model="email"
              type="email"
              autocomplete="username"
              required
            />
          </div>

          <div class="form-group">
            <label for="password">Hasło :</label>
            <input
              id="password"
              v-model="password"
              type="password"
              autocomplete="current-password"
              required
            />
          </div>

          <button
            type="submit"
            class="login-button"
            :disabled="animationStage > 0"
          >
            Zaloguj się
          </button>

          <p v-if="errorMessage" class="error-text">
            {{ errorMessage }}
          </p>
        </form>

        <form
          v-else-if="currentStep === '2fa'"
          key="twofa-form"
          class="login-form"
          :class="{ 'form-hide': animationStage >= 1 }"
          @submit.prevent="verifyTwoFactor"
        >
          <p class="twofa-info">
            Na tym koncie włączono uwierzytelnianie dwuetapowe. Wpisz kod z aplikacji uwierzytelniającej.
          </p>

          <div class="form-group">
            <label for="code">Kod uwierzytelniający :</label>
            <input
              id="code"
              v-model="twoFactorCode"
              type="text"
              inputmode="numeric"
              maxlength="6"
              autocomplete="one-time-code"
              required
            />
          </div>

          <button
            type="submit"
            class="login-button"
            :disabled="animationStage > 0"
          >
            Zweryfikuj kod
          </button>

          <button
            type="button"
            class="secondary-button"
            @click="goBackToLogin"
            :disabled="animationStage > 0"
          >
            Wróć
          </button>

          <p v-if="errorMessage" class="error-text">
            {{ errorMessage }}
          </p>
        </form>
      </transition>
    </div>
  </div>
</template>

<script>
import api from '../api/axios'
import premierLogo from '../assets/logo.png'

export default {
  data() {
    return {
      email: '',
      password: '',
      twoFactorCode: '',
      errorMessage: '',
      premierLogo,
      animationStage: 0,
      currentStep: 'login'
    }
  },
  methods: {
    async login() {
      if (this.animationStage > 0) return

      this.errorMessage = ''

      try {
        const res = await api.post('/Auth/login', {
          email: this.email,
          password: this.password
        })

        if (res.data.requiresTwoFactor) {
          this.currentStep = '2fa'
          this.twoFactorCode = ''
          return
        }

        const token = res.data.token
        sessionStorage.setItem("token", token);

        this.startSuccessAnimation()
      } catch (err) {
        console.error('Login error:', err)
        this.errorMessage =
          err.response?.data?.message ||
          err.response?.data ||
          'Nie udało się zalogować. Sprawdź dane i spróbuj ponownie.'
      }
    },

    async verifyTwoFactor() {
      if (this.animationStage > 0) return

      this.errorMessage = ''

      try {
        const res = await api.post('/Auth/login-2fa', {
          email: this.email,
          code: this.twoFactorCode.replace(/\s+/g, '').replace(/-/g, '')
        })

        const token = res.data.token
        sessionStorage.setItem('token', token)

        this.startSuccessAnimation()
      } catch (err) {
        console.error('2FA error:', err)
        console.log('2FA response:', err.response?.data)

        this.errorMessage =
          err.response?.data?.message ||
          err.response?.data ||
          'Błąd weryfikacji 2FA.'
      }
    },

    startSuccessAnimation() {
      this.animationStage = 1

      setTimeout(() => {
        this.animationStage = 2
      }, 500)

      setTimeout(() => {
        this.animationStage = 3
      }, 1150)

      setTimeout(() => {
        window.dispatchEvent(new Event('auth-changed'))
        this.$router.push('/dashboard')
      }, 1650)
    },

    goBackToLogin() {
      this.currentStep = 'login'
      this.twoFactorCode = ''
      this.errorMessage = ''
    }
  }
}
</script>

<style scoped>
.pl-text,
.app-text,
.form-group label,
.login-button,
.secondary-button,
.twofa-info {
  font-family: 'PremierLeague', Arial, sans-serif;
}

.login-page {
  width: 100vw;
  height: 100vh;
  overflow: hidden;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  position: relative;
}

.page-fade-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
  opacity: 0;
  transition: opacity 0.45s ease;
  pointer-events: none;
  z-index: 5;
}

.page-fade-overlay-show {
  opacity: 1;
}

.login-content {
  width: 100%;
  max-width: 600px;
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  z-index: 2;
}

.branding {
  margin-bottom: 60px;
  transition: transform 0.7s ease, opacity 0.45s ease, filter 0.45s ease;
}

.brand-row {
  display: flex;
  align-items: center;
  gap: 24px;
  justify-content: center;
}

.logo {
  width: 350px;
  height: auto;
}

.brand-text {
  display: flex;
  align-items: center;
  gap: 18px;
  flex-wrap: nowrap;
}

.divider {
  width: 12px;
  height: 100px;
  background: rgb(255, 255, 255);
}

.app-text {
  color: white;
  font-size: 44px;
  font-weight: 700;
  font-family: 'PremierLeague', Arial, sans-serif !important;
}

.login-form {
  width: 100%;
  max-width: 500px;
  display: flex;
  flex-direction: column;
  gap: 26px;
  transition: opacity 0.45s ease, transform 0.45s ease, filter 0.45s ease;
}

.form-switch-enter-active,
.form-switch-leave-active {
  transition: opacity 0.4s ease, transform 0.4s ease, filter 0.4s ease;
}

.form-switch-enter-from {
  opacity: 0;
  transform: translateY(18px);
  filter: blur(6px);
}

.form-switch-leave-to {
  opacity: 0;
  transform: translateY(-18px);
  filter: blur(6px);
}

.form-hide {
  opacity: 0;
  transform: translateY(40px) scale(0.96);
  filter: blur(10px);
  pointer-events: none;
}

.branding-move-down {
  transform: translateY(140px);
}

.branding-fade-out {
  opacity: 0;
  transform: translateY(170px) scale(0.96);
  filter: blur(10px);
}

.twofa-info {
  margin: 0;
  color: white;
  font-size: 18px;
  line-height: 1.4;
  text-align: left;
}

.form-group {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group label {
  width: 100%;
  text-align: left;
  color: white;
  font-size: 22px;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  height: 64px;
  background: #d9d9d9;
  border: none;
  border-radius: 999px;
  color: #1f1f1f;
  font-size: 22px;
  padding: 0 24px;
  outline: none;
}

.login-button {
  margin-top: 20px;
  width: 100%;
  height: 58px;
  border: none;
  border-radius: 999px;
  background: white;
  color: #240a36;
  font-size: 20px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s ease;
}

.login-button:hover {
  transform: translateY(-1px);
  color: #ffffff;
  background-color: #8930b39c;
  box-shadow:
    0 0 5px #960fb1e8,
    0 0 25px #960fb1e8,
    0 0 50px #960fb1e8,
    0 0 200px #960fb1e8;
}

.secondary-button {
  width: 100%;
  height: 58px;
  border: 2px solid white;
  border-radius: 999px;
  background: transparent;
  color: white;
  font-size: 20px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s ease;
}

.secondary-button:hover {
  background: rgba(255, 255, 255, 0.12);
}

.error-text {
  margin: 0;
  color: #ffb4b4;
  font-size: 14px;
}

@media (max-width: 700px) {
  .login-content {
    max-width: 90%;
  }

  .brand-row {
    gap: 16px;
  }

  .logo {
    width: 220px;
  }

  .app-text {
    font-size: 30px;
  }

  .divider {
    height: 64px;
    width: 8px;
  }

  .form-group label,
  .twofa-info {
    font-size: 18px;
  }

  .form-group input {
    font-size: 18px;
    height: 56px;
  }
}
</style>