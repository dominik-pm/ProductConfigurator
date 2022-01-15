import { createSlice } from '@reduxjs/toolkit'

export const alertStatus = {
    CLOSED: 'closed',
    OPEN: 'open'
}

export const alertTypes = {
    ERROR: 'error',
    WARNING: 'warning',
    INFO: 'info',
    SUCCESS: 'success'
}



const initialState = {
    status: alertStatus.CLOSED,
    alerts: [ // can be multiple -> when 1. closed, 2nd pops up
        // {message: '', type: alertTypes.INFO}
    ]
}

export const alertSlice = createSlice({
    name: 'alert',
    initialState,
    reducers: {
        addAlert: (state, action) => {
            state.status = alertStatus.OPEN
            state.alerts.push(action.payload)
        },
        closeAlert: (state) => {
            state.alerts.shift()

            // set the status to closed, when all alerts are gone
            if (state.alerts.length === 0) {
                state.status = alertStatus.CLOSED
            }
        }
    }
})

export const openAlert = (message, type) => async (dispatch) => {
    dispatch(addAlert({ message, type }))
}

// Action creators are generated for each case reducer function
export const { addAlert, closeAlert } = alertSlice.actions

export default alertSlice.reducer