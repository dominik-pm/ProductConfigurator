import React from 'react'
import { Button } from '@mui/material'
import { connect } from 'react-redux'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import { login } from '../../state/user/userSlice'

export const openLogInDialog = () => (dispatch) => {
    const data = {
        username: {name: 'Username', value: ''},
        password: {name: 'Password', value: '', isPassword: true}
    }
    dispatch(inputDialogOpen('Login', data, (data) => {
        dispatch(login(data.username.value, data.username.password))
    }))
}

function LoginButton({ openLogin }) {


    return (
        <Button 
            variant="contained" 
            onClick={openLogin}
            >
            Login
        </Button>
    )
}

const mapStateToProps = (state) => ({
    
})
const mapDispatchToProps = {
    openLogin: openLogInDialog
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LoginButton)