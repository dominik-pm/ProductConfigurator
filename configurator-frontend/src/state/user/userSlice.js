import { createSlice } from '@reduxjs/toolkit'
import { fetchAllOrderedConfigurations, fetchSavedConfigurations, requestLogin, requestRegister, setAuthorizationToken } from '../../api/userAPI'

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
            // console.log('setting saved configurations:', action.payload)
            state.savedConfigurations = action.payload
        },
        setOrderedConfigurations: (state, action) => {
            // console.log('setting ordered configurations:', action.payload)
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

        dispatch(setSavedConfigurations(saved))
        dispatch(setOrderedConfigurations(ordered))
    })
    .catch(err => {
        console.log(err)
        // TODO: display error message
    })
}
export const getAllOrderedConfigurations= () => async (dispatch) => {
    fetchAllOrderedConfigurations()
    .then(configurations => {
        dispatch(setAllOrderedConfigurations(configurations))
    })
    .catch(err => {
        console.log(err)
        // TODO: display error message
    })
}

export const register = (username, password, email) => async (dispatch) => {
    requestRegister(username, password, email).then(res => {
        // TODO: display registered notification
    })
    .catch(err => {
        console.log(err)
        // TODO: display error message
    })
}

export const login = (username, password) => async (dispatch) => {
    
    requestLogin(username, password).then(res => {
        const { token, user } = res

        localStorage.setItem('jwtToken', token)
        setAuthorizationToken(token)
        dispatch(setCurrentUser(user))
        // TODO: display logged in notification
    })
    .catch(err => {
        console.log(err)
        // TODO: display error message
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