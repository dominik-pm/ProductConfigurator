import { Typography } from '@mui/material'
import { Box } from '@mui/system'
import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import { selectIsAdmin, selectIsAuthenticated, selectUserEmail, selectUserName } from '../../state/user/userSelector'
import { getAllOrderedConfigurations, getSavedConfigurations } from '../../state/user/userSlice'
import ConfigurationTabs from './ConfigurationTabs'


function AccountView({ username, email, isLoggedIn, isAdmin, getSavedConfigurations, getAllOrderedConfigurations }) {

    const adminActions = (
        <></>
    )

    const userActions = (
        <></>
    )

    // every time the logged in state changes -> reload the saved configurations
    useEffect(() => {
        if (isLoggedIn) {
            getSavedConfigurations()
        }
    
        if (isAdmin) {
            getAllOrderedConfigurations()
        }
    }, [isLoggedIn, isAdmin, getSavedConfigurations, getAllOrderedConfigurations])

    
    function renderUserActions() {
        return (
            <Box>
                {userActions}
                {isAdmin ? adminActions : ''}
            </Box>
        )
    }

    function renderUserDetails() {
        return (
            <Box className="userdetails" display="flex" flexDirection="column" alignItems="center">
                <Typography variant="h2">{username}</Typography>
                <Typography variant="body1">{email}</Typography>
            </Box>
        )
    }

    function renderLoggedIn() {
        return (
            <>
                {renderUserDetails()}
                {renderUserActions()}
                <ConfigurationTabs></ConfigurationTabs>
            </>
        )
    }

    function renderLoggedOut() {
        return (
            <Box>
                <Typography variant="h2">Log in to access this page!</Typography>
            </Box>
        )
    }

    return (
        <Box className="accountpage" display="flex" flexDirection="column" alignItems="center">
            {isLoggedIn ? renderLoggedIn() : renderLoggedOut()}
        </Box>
    )
}

const mapStateToProps = (state) => ({
    isLoggedIn: selectIsAuthenticated(state),
    isAdmin: selectIsAdmin(state),
    username: selectUserName(state),
    email: selectUserEmail(state)
})
const mapDispatchToProps = {
    getSavedConfigurations: getSavedConfigurations,
    getAllOrderedConfigurations: getAllOrderedConfigurations
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(AccountView)
