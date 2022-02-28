import { createSlice } from '@reduxjs/toolkit'
import { selectInputDialogData, selectIsInputDialogOpen } from './inputDialogSelectors'

const initialState = {
    open: false,
    headerMessage: '',
    data: {}
}

export const inputDialogSlice = createSlice({
    name: 'inputDialog',
    initialState,
    reducers: {
        show: (state, action) => {
            const { headerMessage, data } = action.payload

            // console.log('Opened input dialog:', data)
            state.headerMessage = headerMessage
            state.data = data
            state.open = true
        },
        close: (state, action) => {
            // console.log('Closed input dialog')
            state.open = false
            state.headerMessage = ''
            state.data = {}
        },
        setData: (state, action) => {
            // console.log('Set input dialog data:', action.payload)
            state.data = action.payload
        }
    }
})


let onConfirm = null

export const inputDialogOpen = (headerMessage, data, onConfirmCallback) => (dispatch, getState) => {
    const isOpen = selectIsInputDialogOpen(getState())
    if (isOpen) {
        console.log('Input Dialog is already open!')
        return
    }

    onConfirm = onConfirmCallback
    
    dispatch(show({headerMessage, data}))
}

export const inputDialogConfirm = () => (dispatch, getState) => {
    const isOpen = selectIsInputDialogOpen(getState())
    if (!isOpen) {
        console.log('Input Dialog is closed (can not confirm)!')
        return
    }

    if (onConfirm) {
        const data = selectInputDialogData(getState())
        onConfirm(data)
    } else {
        console.log('no confirmation callback')
    }
    dispatch(close())
    onConfirm = null
}

export const inputDialogCancel = () => (dispatch) => {
    dispatch(close())
    onConfirm = null
}

export const inputDialogSetData = (data) => (dispatch) => {
    dispatch(setData(data))
}


// Action creators are generated for each case reducer function
export const { show, close, setData } = inputDialogSlice.actions

export default inputDialogSlice.reducer