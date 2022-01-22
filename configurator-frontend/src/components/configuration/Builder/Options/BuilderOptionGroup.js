import { IconButton, Tooltip, Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { translate } from '../../../../lang'
import { connect } from 'react-redux'
import BuilderOption from './BuilderOption'
import { Add } from '@mui/icons-material'
import { alertTypes, openAlert } from '../../../../state/alert/alertSlice'
import { createOption } from '../../../../state/configurationBuilder/builderSlice'
import { inputDialogOpen } from '../../../../state/inputDialog/inputDialogSlice'

function OptionGroup({group, createOption, openInputDialog, openAlert, language}) {

    const { name, description, optionIds } = group


    function handleAddOption() {
        const data = {
            optionName: {name: 'name', value: '' },
            optionDescription: {name: 'description', value: ''},
        }
        openInputDialog(translate('newOption', language), data, (data) => {
            const success = createOption(group.id, data.optionName.value, data.optionDescription.value)
            if (!success) {
                openAlert('Option already exists!', alertTypes.ERROR)
            }
        })
    }

    return (
        <Box marginBottom={1} padding={1}>
            <Box display={'flex'} justifyContent={'space-between'} alignItems={'center'}>
                <Typography variant="h3">
                    {name}
                </Typography>
            </Box>
            <Typography variant="subtitle2">{description}</Typography>
            <Box>
                <Tooltip title={translate('addOption', language)}>
                    <IconButton onClick={handleAddOption}>
                        <Add />
                    </IconButton>
                </Tooltip>
            </Box>
            <Box>
                {optionIds.map(optionId => (
                    <BuilderOption key={optionId} optionId={optionId}></BuilderOption>
                ))}
            </Box>
        </Box>
    )
}

const mapStateToProps = (state, ownProps) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    createOption,
    openInputDialog: inputDialogOpen,
    openAlert
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(OptionGroup)
