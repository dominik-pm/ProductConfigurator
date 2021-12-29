import { createSlice } from '@reduxjs/toolkit'
import { selectIsConfirmDialogOpen } from './confirmationSelectors'

const initialState = {
    open: false,
    message: '',
    content: {}
}

export const confirmationSlice = createSlice({
    name: 'confirmation',
    initialState,
    reducers: {
        show: (state, action) => {
            const { message, content } = action.payload

            // console.log('Opened confirmation dialog: ' + message)
            state.open = true
            state.message = message
            state.content = content
        },
        close: (state, action) => {
            // console.log('Closed confirmation dialog')
            state.open = false
            state.message = ''
            state.content = {}
        }
    }
})


let onConfirm = null

export const dialogOpen = (message, content, onConfirmCallback) => (dispatch, getState) => {
    const isOpen = selectIsConfirmDialogOpen(getState())
    if (isOpen) {
        console.log('Confirmation Dialog is already open!')
        return
    }

    onConfirm = onConfirmCallback
    
    dispatch(show({message, content}))
}

export const dialogConfirm = () => (dispatch, getState) => {
    const isOpen = selectIsConfirmDialogOpen(getState())
    if (!isOpen) {
        console.log('Confirmation Dialog is closed (can not confirm)!')
        return
    }

    dispatch(close())
    if (onConfirm) {
        onConfirm()
    } else {
        console.log('no confirmation callback')
    }
    onConfirm = null
}

export const dialogCancel = () => (dispatch) => {
    dispatch(close())
    onConfirm = null
}

// class confirmationDialog {
//     constructor() {
//         this.onConfirm = null
//     }

//     open = (message, content, onConfirm) => (dispatch, getState) => {
//         const isOpen = selectIsConfirmDialogOpen(getState())
//         if (isOpen) {
//             console.log('Confirmation Dialog is already open!')
//             return
//         }

//         this.onConfirm = onConfirm
        
//         dispatch(show({message, content}))
//     }

//     confirm = () => (dispatch, getState) => {
//         const isOpen = selectIsConfirmDialogOpen(getState())
//         if (!isOpen) {
//             console.log('Confirmation Dialog is closed (can not confirm)!')
//             return
//         }
    
//         dispatch(close())
//         if (this.onConfirm) {
//             this.onConfirm()
//         } else {
//             console.log('no confirmation callback')
//         }
//         this.onConfirm = null
//     }

//     cancel = () => (dispatch) => {
//         dispatch(close())
//         this.onConfirm = null
//     }
// }

// export const useConfirmationDialog = new confirmationDialog() 

// export const openConfirmDialog = (message) => (dispatch, getState) => {
//     const isOpen = selectIsConfirmDialogOpen(getState())
//     if (isOpen) {
//         console.log('Confirmation Dialog is already open!')
//         return
//     }

//     dispatch(show({message}))
// }
// export const closeConfirmDialog = (isConfirmed = false) => (dispatch, getState) => {
//     const isOpen = selectIsConfirmDialogOpen(getState())
//     if (!isOpen) {
//         console.log('Confirmation Dialog is already closed!')
//         return
//     }

//     if (isConfirmed) {
//         getState().confirmation.onConfirm()
//         dispatch(confirm())
//     } else {
//         dispatch(cancel())
//     }
// }

// Action creators are generated for each case reducer function
export const { show, close/*, confirm, cancel*/ } = confirmationSlice.actions

export default confirmationSlice.reducer