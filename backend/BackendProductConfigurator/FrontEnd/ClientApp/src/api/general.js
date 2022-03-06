import axios from 'axios'

export const baseURL = process.env.REACT_APP_BACKEND_URL || 'localhost:7187' // 'localhost:3000'
export const LOCAL_DATA = process.env.REACT_APP_PRODUCTION === 'false' // true

axios.defaults.timeout = 5000

export const setAuthorizationToken = (token) => {
    if (token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
    } else {
        delete axios.defaults.headers.common['Authorization']
    }
}

export const setAcceptLanguage = (lang) => {
    if (lang) {
        axios.defaults.headers.common['Accept-Language'] = lang
    } else {
        delete axios.defaults.headers.common['Accept-Language']
    }
}