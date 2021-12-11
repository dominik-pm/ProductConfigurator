import { configureStore } from '@reduxjs/toolkit'
import productReducer from './product/productSlice'
import configurationReducer from './configuration/configurationSlice'

export const store = configureStore({
    reducer: {
        product: productReducer,
        configuration: configurationReducer
    }
})