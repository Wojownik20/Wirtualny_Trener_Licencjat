<template>
  <div id="app">
    <AppNavbar
  v-if="isLoggedIn && $route.path !== '/' && $route.path !== '/login'"
  :team="team"
  :teamLogoUrl="teamLogoUrl"
  @logout="logout"
/>

    <router-view />
  </div>
</template>

<script>
import api from './api/axios'
import AppNavbar from './components/AppNavbar.vue'

export default {
  components: {
    AppNavbar
  },

  data() {
  return {
    isLoggedIn: !!sessionStorage.getItem('token'),
    team: null,
    teamLogoUrl: null
  }
},

  async mounted() {
    window.addEventListener('auth-changed', this.updateAuth)

    if (this.isLoggedIn) {
      await this.fetchTeam()
    }
  },

  beforeUnmount() {
    window.removeEventListener('auth-changed', this.updateAuth)
  },

  methods: {
    async updateAuth() {
  this.isLoggedIn = !!sessionStorage.getItem('token')

  if (this.isLoggedIn) {
    await this.fetchTeam()
  } else {
    this.team = null
    this.teamLogoUrl = null
  }
},

    async fetchTeam() {
  const res = await api.get('/Staff/me')
  this.team = res.data.team
  this.teamLogoUrl = res.data.teamLogoUrl
},

    logout() {
  sessionStorage.removeItem('token')
  this.isLoggedIn = false
  this.team = null
  this.teamLogoUrl = null
  window.dispatchEvent(new Event('auth-changed'))
  this.$router.push('/')
}
  }
}
</script>

<style>
* {
  box-sizing: border-box;
}

html,
body,
#app {
  margin: 0;
  padding: 0;
  width: 100%;
  min-height: 100%;
  overflow-y: auto;
}

body {
  overflow-x: hidden;
  font-family: 'PremierLeague', sans-serif;
  background: linear-gradient(180deg, #5b0b73 0%, #32003f 100%);
}

#app {
  min-height: 100vh;
  height: auto;
}
</style>