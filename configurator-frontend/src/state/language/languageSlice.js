import { createSlice } from '@reduxjs/toolkit'

export const defaultLang = 'EN'
const localStorageLang = localStorage.getItem('language')

const initialState = {
    language: localStorageLang ? localStorageLang : defaultLang,
    // status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const languageSlice = createSlice({
    name: 'language',
    initialState,
    reducers: {
        changedLanguage: (state, action) => {
            console.log('Changed language to: ' + action.payload)
            state.language = action.payload
        }
    }
})

export const setLanguage = (lang) => (dispatch) => {
    console.log('dispatching language change to: ' + lang)
    localStorage.setItem('language', lang)
    dispatch(changedLanguage(lang))
}

// Action creators are generated for each case reducer function
export const { changedLanguage } = languageSlice.actions

export default languageSlice.reducer