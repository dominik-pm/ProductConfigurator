import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'
import configurationReducer from './configuration/configurationSlice'
import languageReducer, { setLanguage } from './language/languageSlice'
import confirmationReducer from './confirmationDialog/confirmationSlice'
import userReducer, { setCurrentUser } from './user/userSlice'
import inputDialogReducer from './inputDialog/inputDialogSlice'
import jwt from 'jsonwebtoken'
import { setAuthorizationToken } from '../api/general'

export const store = configureStore({
    reducer: {
        product: productReducer,
        configuration: configurationReducer,
        language: languageReducer,
        confirmation: confirmationReducer,
        user: userReducer,
        inputDialog: inputDialogReducer
    }
})

const authToken = localStorage.jwtToken
if (authToken) {
    setAuthorizationToken(authToken)
    store.dispatch(setCurrentUser(jwt.decode(authToken)))
}

const localStorageLang = localStorage.language
if (localStorageLang) {
    console.log('storage lang:', localStorageLang)
    store.dispatch(setLanguage(localStorageLang))
}