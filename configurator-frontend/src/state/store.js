import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'
import configurationReducer from './configuration/configurationSlice'
import languageReducer from './language/languageSlice'

export const store = configureStore({
    reducer: {
        product: productReducer,
        configuration: configurationReducer,
        language: languageReducer
    }
})