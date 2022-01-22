import axios from 'axios'

export const baseURL = `https://sqrt3.ddns.net:7187`
// export const baseURL = ``

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