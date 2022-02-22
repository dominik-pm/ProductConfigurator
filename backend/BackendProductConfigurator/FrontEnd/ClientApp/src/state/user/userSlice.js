import { createSlice } from '@reduxjs/toolkit'
import { setAuthorizationToken } from '../../api/general'
import { fetchAllOrderedConfigurations, fetchSavedConfigurations, requestLogin, requestRegister } from '../../api/userAPI'
import { alertTypes, openAlert } from '../alert/alertSlice'

const initialState = {
    isAuthenticated: false,
    user: {},
    savedConfigurations: [],
    orderedConfigurations: [],
    allOrderedConfigurations: []
}

export const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setCurrentUser: (state, action) => {
            console.log('getting user:', action.payload)
            state.user = action.payload
            if (Object.keys(action.payload).length > 0) {
                // user is set
                state.isAuthenticated = true
            } else {
                // user is removed
                state.isAuthenticated = false
                state.savedConfigurations = []
                state.orderedConfigurations = []
                state.allOrderedConfigurations = []
            }
        },
        setSavedConfigurations: (state, action) => {
            console.log('setting saved configurations:', action.payload)
            state.savedConfigurations = action.payload
        },
        setOrderedConfigurations: (state, action) => {
            console.log('setting ordered configurations:', action.payload)
            state.orderedConfigurations = action.payload
        },
        setAllOrderedConfigurations: (state, action) => {
            // console.log('setting all ordered configurations:', action.payload)
            state.allOrderedConfigurations = action.payload
        }
    }
})

export const getSavedConfigurations = () => async (dispatch) => {
    fetchSavedConfigurations()
    .then(configurations => {
        let saved = configurations.filter(config => config.status === 'saved')
        let ordered = configurations.filter(config => config.status === 'ordered')

        if (saved.length > 0) dispatch(setSavedConfigurations(saved))
        if (ordered.length > 0) dispatch(setOrderedConfigurations(ordered))
    })
    .catch(err => {
        console.log(err)
        dispatch(openAlert(err, alertTypes.ERROR))
    })
}
export const getAllOrderedConfigurations= () => async (dispatch) => {
    fetchAllOrderedConfigurations()
    .then(configurations => {
        dispatch(setAllOrderedConfigurations(configurations))
    })
    .catch(err => {
        console.log(err)
        dispatch(openAlert(err, alertTypes.ERROR))
    })
}

export const register = (username, password, email) => async (dispatch) => {
    requestRegister(username, password, email).then(res => {
        dispatch(openAlert('Registered!', alertTypes.SUCCESS))
    })
    .catch(err => {
        dispatch(openAlert(err, alertTypes.ERROR))
    })
}

export const login = (username, password) => async (dispatch) => {
    
    requestLogin(username, password).then(res => {
        const { token, user } = res

        localStorage.setItem('jwtToken', token)
        setAuthorizationToken(token)
        dispatch(setCurrentUser(user))
        dispatch(openAlert('Logged In!', alertTypes.SUCCESS))
    })
    .catch(err => {
        dispatch(openAlert(err, alertTypes.ERROR))
    })
}

export const logout = () => (dispatch) => {
    localStorage.removeItem('jwtToken')
    setAuthorizationToken(false)
    dispatch(setCurrentUser({}))
}

// Action creators are generated for each case reducer function
export const { setCurrentUser, setSavedConfigurations, setOrderedConfigurations, setAllOrderedConfigurations } = userSlice.actions

export default userSlice.reducer