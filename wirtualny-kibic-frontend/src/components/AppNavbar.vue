<template>
  <nav class="app-navbar" :style="navbarStyle">
    <div class="navbar-container">
      <div class="nav-left">
        <router-link to="/dashboard" class="nav-item">Home</router-link>
        <router-link to="/players" class="nav-item">Skład</router-link>
        <router-link to="/formations" class="nav-item">Formacje</router-link>
        <router-link to="/statistics" class="nav-item">Statystyki</router-link>
        <router-link to="/fixtures" class="nav-item">Wyniki</router-link>
        <router-link to="/profile" class="nav-item">Profil</router-link>
      </div>

      <div class="nav-right">
      
        <div class="brand-block">
          <img
  v-if="teamLogoUrl"
  :src="teamLogoUrl"
  class="team-logo"
/>
          <img
            src="../Assets/logo.png"
            alt="Premier League"
            class="pl-logo"
          />

          <div class="divider"></div>

          <div class="brand-text">
            <span class="brand-main">Wirtualny</span>
            <span class="brand-accent">Trener</span>
          </div>
        </div>

        <button class="logout" @click="$emit('logout')">Logout</button>
      </div>
    </div>
  </nav>
</template>

<script>
export default {
  name: 'AppNavbar',

  props: {
  team: {
    type: String,
    default: null
  },
  teamLogoUrl: {
    type: String,
    default: null
  }
},

  computed: {
    navbarStyle() {
      const colors = {
        Arsenal: 'linear-gradient(90deg, #5f0000, #db0007)',
        'Aston Villa': 'linear-gradient(90deg, #480c0c, #c98f25)',
        Bournemouth: 'linear-gradient(90deg, #d80808, #5c1313)',
        Brentford: 'linear-gradient(90deg, #c98f25, #c40909)',
        'Brighton & Hove Albion': 'linear-gradient(90deg, #7d70d3, #073a85)',
        Burnley: 'linear-gradient(90deg, #6d2121, #430626)',
        Chelsea: 'linear-gradient(90deg, #020f5e, #016be4)',
        'Crystal Palace': 'linear-gradient(90deg, #67a2e6, #4e0522)',
        Everton: 'linear-gradient(90deg, #2011c7, #09034f)',
        Fulham: 'linear-gradient(90deg, #979a9e, #1a0202)',
        Liverpool: 'linear-gradient(90deg, #490101, #c8102e)',
        'Manchester City': 'linear-gradient(90deg, #66aee9, #232e50)',
        'Manchester United': 'linear-gradient(90deg, #680b0b, #dd1b0d)',
        'Newcastle United': 'linear-gradient(90deg, #b3a9a9, #141121)',
        'Nottingham Forest': 'linear-gradient(90deg, #ef3232, #550404)',
        Sunderland: 'linear-gradient(90deg, #810a19, #8f0018)',
        'Tottenham Hotspur': 'linear-gradient(90deg, #121825, #16308c)',
        'West Ham United': 'linear-gradient(90deg, #8e3400, #481800)',
        'Wolverhampton Wanderers': 'linear-gradient(90deg, #f98500, #713f05)',
        'Leeds United': 'linear-gradient(90deg, #c59805, #16308c)'
      }

      return {
        background: colors[this.team] || 'linear-gradient(90deg, #3a004d, #5b0b73)',
        borderBottom: '3px solid rgba(255,255,255,0.16)'
      }
    }
  }
}

</script>

<style scoped>
.app-navbar {
  width: 100%;
  min-height: 72px;
  font-family: 'PremierLeague', sans-serif;
}

.navbar-container {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 24px;
  min-height: 72px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
}

.nav-left {
  display: flex;
  align-items: center;
  gap: 34px;
  flex-wrap: wrap;
}

.nav-item {
  color: white;
  text-decoration: none;
  font-weight: 800;
  font-size: 21px;
  line-height: 1;
  position: relative;
}

.nav-item:hover {
  opacity: 0.5;
}

.nav-item.router-link-active::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: -10px;
  width: 100%;
  height: 3px;
  background: #ffffff;
}

.nav-right {
  display: flex;
  align-items: center;
  gap: 22px;
}

.brand-block {
  display: flex;
  align-items: center;
  gap: 16px;
}

.team-logo {
  display: block;
  width: 50px;
  height: 50px;
  object-fit: contain;
}

.pl-logo {
  display: block;
  height: 50px;
  width: auto;
  max-width: 165px;
  object-fit: contain;
}

.divider {
  width: 2px;
opacity: 0.6;
  height: 34px;
  background: rgb(255, 255, 255);
}

.brand-text {
  display: flex;
  flex-direction: column;
  justify-content: center;
  line-height: 0.95;
}

.brand-main {
  color: white;
  font-weight: 900;
  font-size: 24px;
}

.brand-accent {
  color: #ffffff;
  font-weight: 800;
  font-size: 25px;
}

.logout {
  border: none;
  font-family: 'PremierLeague', sans-serif;
  background: rgba(255, 255, 255, 0.16);
  color: white;
  font-size: 16px;
  border-radius: 12px;
  padding: 11px 18px;
  cursor: pointer;
}

.logout:hover {
  background: rgba(255, 255, 255, 0.25);
}

@media (max-width: 980px) {
  .navbar-container {
    padding: 14px 18px;
    flex-direction: column;
    align-items: flex-start;
  }

  .nav-right {
    width: 100%;
    justify-content: space-between;
  }
}
</style>