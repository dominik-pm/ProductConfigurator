import { createSlice } from '@reduxjs/toolkit'
import { requestLogin, requestRegister, setAuthorizationToken } from '../../api/userAPI'

const initialState = {
    isAuthenticated: false,
    user: {}
}

export const userSlice = createSlice({
    name: 'user',
    initialState,
    reducers: {
        setCurrentUser: (state, action) => {
            console.log('getting user:', action.payload)
            state.isAuthenticated = Object.keys(action.payload).length > 0
            state.user = action.payload
        }
    }
})

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
export const { setCurrentUser } = userSlice.actions

export default userSlice.reducer