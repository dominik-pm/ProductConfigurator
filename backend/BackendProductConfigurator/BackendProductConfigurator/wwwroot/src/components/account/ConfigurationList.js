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

function ConfigurationList({ configurations, openConfirm, isOrdered = false, isAdminView = false, openAlert, language }) {

    const navigate = useNavigate()

    function handleEditClick(id, options) {
        navigate(`/configuration/${id}`)
        saveConfigurationToStorage(id, options)
    }

    function handleDeleteClicked(id, name) {
        requestDeleteSavedConfiguration(id, name)
        .then(res => {
            openAlert(`${translate('successfullyRemoved', language)} ${name}!`, alertTypes.SUCCESS)
            // refresh
            navigate(`/account/saved`)
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
                    return (
                        <Box key={index} margin={2}>
                            <Box display="flex" alignItems="center">
                                <Typography variant="body1">{config.savedName}</Typography>
                                <IconButton onClick={() => handleEditClick(config.id, config.options)}>
                                    <Edit></Edit>
                                </IconButton>
                                {!isOrdered ? 
                                <IconButton onClick={() => handleDeleteClicked(config.id, config.savedName)}>
                                    <Delete></Delete>
                                </IconButton>
                                : ''}
                                {isOrdered ?
                                <IconButton onClick={() => handleShowSummaryClicked(config.id, config.options)}>
                                    <Preview></Preview>
                                </IconButton>
                                : ''}
                            </Box>

                            <Typography variant="body2">{config.name}</Typography>

                            <Typography variant="body2">{new Date(config.date).toLocaleTimeString()}</Typography>

                            {isAdminView ? 
                            <Typography variant="body2">From: {config.userName}</Typography>
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