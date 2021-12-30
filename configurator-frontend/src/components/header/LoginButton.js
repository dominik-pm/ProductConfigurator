import React from 'react'
import { Button } from '@mui/material'
import { connect } from 'react-redux'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import { login } from '../../state/user/userSlice'

function LoginButton({ openInputDialog, login }) {

    function openLogInDialog() {
        const data = {
            username: {name: 'Username', value: ''},
            password: {name: 'Password', value: '', isPassword: true}
        }
        openInputDialog('Login', data, (data) => {
            login(data.username.value, data.username.password)
        })
    }

    return (
        <Button 
            variant="contained" 
            onClick={openLogInDialog}
            >
            Login
        </Button>
    )
}

const mapStateToProps = (state) => ({
    
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    login: login
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LoginButton)