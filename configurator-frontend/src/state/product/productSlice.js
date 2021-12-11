import { createSlice } from '@reduxjs/toolkit'

const initialState = {
    products: [],
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const productSlice = createSlice({
    name: 'counter',
    initialState,
    reducers: {
        loadingStarted: (state) => {
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            state.status = 'succeeded'
            state.products = action.payload
        },
        loadingFailed: (state, action) => {
            state.status = 'failed'
            state.eroor = action.payload
        }
    }
})

// Action creators are generated for each case reducer function
export const { loadingStarted, loadingSucceeded, loadingFailed } = productSlice.actions

export default productSlice.reducer