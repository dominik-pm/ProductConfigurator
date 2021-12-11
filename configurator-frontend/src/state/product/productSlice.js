import { createSlice } from '@reduxjs/toolkit'
import { fetchAll } from '../../api/productsAPI'

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
            console.log('fetching products...')
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            console.log('products loaded:', action.payload)
            state.status = 'succeeded'
            state.products = action.payload
        },
        loadingFailed: (state, action) => {
            console.log('products loading failed:', action.payload)
            state.status = 'failed'
            state.error = action.payload
        }
    }
})

export const fetchProducts = () => async (dispatch) => {
    dispatch(loadingStarted())

    fetchAll()
    .then(res => {
        dispatch(loadingSucceeded(res.products))
    })
    .catch(err => {
        dispatch(loadingFailed(err))
    })
}

// Action creators are generated for each case reducer function
export const { loadingStarted, loadingSucceeded, loadingFailed } = productSlice.actions

export default productSlice.reducer