import React from 'react'
import { connect } from 'react-redux'
import { Box, Grid, IconButton, Typography } from '@mui/material'
import { Delete, Edit, Preview } from '@mui/icons-material'
import { useNavigate } from 'react-router-dom'
import { saveConfigurationToStorage } from '../../state/configuration/configurationSlice'
import Summary from '../configuration/Configurator/SidePanel/Summary'
import { confirmDialogOpen } from '../../state/confirmationDialog/confirmationSlice'
import { requestDeleteSavedConfiguration } from '../../api/userAPI'
import { alertTypes, openAlert } from '../../state/alert/alertSlice'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { extractDateFromConfiguration, extractIdFromConfiguration, extractNameFromConfiguration, extractOptionsFromConfiguration, extractUsernameFromConfiguration } from '../../state/user/userSelector'

function ConfigurationList({ configurations, openConfirm, isOrdered = false, isAdminView = false, openAlert, language }) {

    const navigate = useNavigate()

    function handleEditClick(id, options) {
        saveConfigurationToStorage(id, options)
        navigate(`/configurator/${id}`)
    }

    function handleDeleteClicked(id, name) {
        requestDeleteSavedConfiguration(id, name)
        .then(res => {
            openAlert(`${translate('successfullyRemoved', language)} ${name}!`, alertTypes.SUCCESS)
            // refresh
            navigate(`/user/saved`)
        })
        .catch(err => {
            openAlert(`Error: ${err}`, alertTypes.ERROR)
        })
    }

    function handleShowSummaryClicked(id, options) {
        openConfirm('', {}, <Summary configurationId={id} selectedOptions={options}></Summary>, () => {

        })
    }

    return (
        <Box>
            <Grid container justifyContent="center">
                {configurations.map((config, index) => {
                    const id = extractIdFromConfiguration(config)
                    const options = extractOptionsFromConfiguration(config)
                    const name = extractNameFromConfiguration(config)
                    const date = extractDateFromConfiguration(config)
                    const username = extractUsernameFromConfiguration(config)

                    return (
                        <Box key={index} margin={2}>
                            <Box display="flex" alignItems="center">
                                <Typography variant="body1">{name}</Typography>
                                <IconButton onClick={() => handleEditClick(id, options)}>
                                    <Edit></Edit>
                                </IconButton>
                                {!isOrdered ? 
                                <IconButton onClick={() => handleDeleteClicked(id, name)}>
                                    <Delete></Delete>
                                </IconButton>
                                : ''}
                                {isOrdered ?
                                <IconButton onClick={() => handleShowSummaryClicked(id, options)}>
                                    <Preview></Preview>
                                </IconButton>
                                : ''}
                            </Box>

                            <Typography variant="body2">{name}</Typography>

                            <Typography variant="body2">{new Date(date).toLocaleTimeString()}</Typography>

                            {isAdminView ? 
                            <Typography variant="body2">From: {username}</Typography>
                            : ''}
                        </Box>
                    )
                })}
            </Grid>
        </Box>
    )
}

const mapStateToProps = (state) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    openConfirm: confirmDialogOpen,
    openAlert: openAlert
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ConfigurationList)