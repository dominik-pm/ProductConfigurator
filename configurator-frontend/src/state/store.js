import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'
import configurationReducer from './configuration/configurationSlice'
import languageReducer from './language/languageSlice'
import confirmationReducer from './confirmationDialog/confirmationSlice'
import userReducer, { setCurrentUser } from './user/userSlice'
import { setAuthorizationToken } from '../api/userAPI'
import jwt from 'jsonwebtoken'

export const store = configureStore({
    reducer: {
        product: productReducer,
        configuration: configurationReducer,
        language: languageReducer,
        confirmation: confirmationReducer,
        user: userReducer
    }
})

const authToken = localStorage.jwtToken
if (authToken) {
    setAuthorizationToken(authToken)
    store.dispatch(setCurrentUser(jwt.decode(authToken)))
}