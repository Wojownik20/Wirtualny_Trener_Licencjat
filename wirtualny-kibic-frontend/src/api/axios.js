import axios from 'axios'
import router from '../router'

const api = axios.create({
  baseURL: 'https://localhost:7133/api'
})

api.interceptors.request.use(config => {
  const token = sessionStorage.getItem('token')

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

api.interceptors.response.use(
  response => response,
  error => {
    console.error('API ERROR:', error)

    if (!error.response) {
      alert('Backend is offline!')
      sessionStorage.removeItem('token')
      localStorage.removeItem('token')
      router.push('/')
    }

    if (error.response?.status === 401) {
      sessionStorage.removeItem('token')
      localStorage.removeItem('token')
      router.push('/')
    }

    return Promise.reject(error)
  }
)

export default api