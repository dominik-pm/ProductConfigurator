import { createSlice } from '@reduxjs/toolkit'
import { setAcceptLanguage } from '../../api/general'
import { defaultLang, languageNames } from '../../lang'


const initialState = {
    language: defaultLang
    // status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    // error: null
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
    // check if the language exists
    if (!Object.values(languageNames).includes(lang)) {
        console.log(`language '${lang}' does not exist!`)
        lang = defaultLang
    }

    localStorage.setItem('language', lang)
    setAcceptLanguage(lang)
    dispatch(changedLanguage(lang))
}

// Action creators are generated for each case reducer function
export const { changedLanguage } = languageSlice.actions

export default languageSlice.reducer