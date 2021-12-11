import { createSlice } from '@reduxjs/toolkit'
import { fetchId } from '../../api/configurationAPI'

const initialState = {
    configuration: {},
    selectedOptions: [],
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const configurationSlice = createSlice({
    name: 'configuration',
    initialState,
    reducers: {
        selectOption: (state, action) => {
            if (state.selectedOptions.includes(action.payload)) {
                // deselect option
                state.selectedOptions = state.selectedOptions.filter(optionId => optionId !== action.payload)
            } else {
                // select option
                state.selectedOptions.push(action.payload)
            }
        },
        loadingStarted: (state) => {
            console.log('fetching configuration...')
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            console.log('configuration loaded:', action.payload)
            state.status = 'succeeded'
            state.configuration = action.payload
            state.selectedOptions = action.payload.rules.defaultOptions
        },
        loadingFailed: (state, action) => {
            console.log('configuration loading failed:', action.payload)
            state.status = 'failed'
            state.error = action.payload
            state.configuration = {}
            state.selectedOptions = []
        }
    }
})


export const fetchConfiguration = (id) => async (dispatch) => {
    dispatch(loadingStarted())

    fetchId(id)
    .then(res => {
        dispatch(loadingSucceeded(res))
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}

// Action creators are generated for each case reducer function
export const { selectOption, loadingStarted, loadingSucceeded, loadingFailed } = configurationSlice.actions

export default configurationSlice.reducer