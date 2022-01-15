import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'
import configurationReducer from './configuration/configurationSlice'
import languageReducer, { setLanguage } from './language/languageSlice'
import confirmationReducer from './confirmationDialog/confirmationSlice'
import userReducer, { setCurrentUser } from './user/userSlice'
import inputDialogReducer from './inputDialog/inputDialogSlice'
import alertReducer from './alert/alertSlice'
import builderReducer from './configurationBuilder/builderSlice'
import jwt from 'jsonwebtoken'
import { setAuthorizationToken } from '../api/general'

export const store = configureStore({
    reducer: {
        product: productReducer,
        configuration: configurationReducer,
        language: languageReducer,
        confirmation: confirmationReducer,
        user: userReducer,
        inputDialog: inputDialogReducer,
        alert: alertReducer,
        builder: builderReducer
    }
})

// set user if there is a user token saved to the local storage
const authToken = localStorage.jwtToken
if (authToken) {
    setAuthorizationToken(authToken)
    store.dispatch(setCurrentUser(jwt.decode(authToken)))
}

// set language if there is a language saved to the local storage
const localStorageLang = localStorage.language
if (localStorageLang) {
    console.log('storage lang:', localStorageLang)
    store.dispatch(setLanguage(localStorageLang))
}