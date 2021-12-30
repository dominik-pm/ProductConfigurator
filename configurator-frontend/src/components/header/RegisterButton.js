import React from 'react'
import { Button } from '@mui/material'
import { connect } from 'react-redux'
import { inputDialogOpen } from '../../state/inputDialog/inputDialogSlice'
import { register } from '../../state/user/userSlice'

function LoginButton({ openInputDialog, register }) {

    function openRegisterDialog() {
        const data = {
            username: {name: 'Username', value: ''},
            email: {name: 'Email', value: '', isEmail: true},
            password: {name: 'Password', value: '', isPassword: true},
            confirmPassword: {name: 'Confirm Password', value: '', isPassword: true}
        }
        openInputDialog('Register', data, (data) => {
            if (data.confirmPassword.value !== data.password.value) {
                console.log('Passwords do not math!')
                // TODO: display notification that password do not match
                return
            }
            register(data.username.value, data.email.value, data.username.password)
        })
    }

    return (
        <Button 
            variant="contained" 
            onClick={openRegisterDialog}
            >
            Register
        </Button>
    )
}

const mapStateToProps = (state) => ({
    
})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen,
    register: register
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LoginButton)