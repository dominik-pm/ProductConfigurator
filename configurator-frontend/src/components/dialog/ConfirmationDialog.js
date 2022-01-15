import React from 'react'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import DialogActions from '@mui/material/DialogActions'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogTitle from '@mui/material/DialogTitle'
import { selectConfirmDialogData, selectConfirmDialogMessage, selectIsConfirmDialogOpen } from '../../state/confirmationDialog/confirmationSelectors'
import { connect } from 'react-redux'
import { confirmDialogCancel, confirmDialogConfirm, confirmDialogGetBody } from '../../state/confirmationDialog/confirmationSlice'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { Grid, Typography } from '@mui/material'
import OptionListItem from '../configuration/Configurator/Options/OptionListItem'
import { Box } from '@mui/system'

function ConfirmationOptionSelect({ isOpen, message, content, optionsToSelect, optionsToRemove, selectedOption, deselectedOption, cancel, confirm, text }) {
    
    if (!optionsToSelect) optionsToSelect = []
    if (!optionsToRemove) optionsToRemove = []

    function handleClose() {
        cancel()
    }

    function handleConfirm() {
        confirm()
    }

    function renderDialogContent() {
        if (optionsToSelect.length > 0 || optionsToRemove.length > 0) {
            return (
                <div>
                    <Box marginBottom={2}>
                        {renderDialogContentHeader()}
                    </Box>
                    {renderDialogContentOptions(optionsToSelect)}
                    {renderDialogContentOptions(optionsToRemove, false)}
                </div>
            )
        }
        return (
            <>
                <DialogContentText>
                    {message}
                </DialogContentText>
                {content}
            </>
        )
    }
    function renderDialogContentHeader() {
        return (
            <>
                <Typography variant="body1">
                    {selectedOption ? 
                        `${text.youWantToSelect}: `
                        :
                        `${text.youWantToRemove}: `
                    }

                </Typography>
                <Grid container justifyContent="center">
                    <OptionListItem optionId={selectedOption || deselectedOption} highlight="add"></OptionListItem>
                </Grid>
            </>
        )
    }
    function renderDialogContentOptions(options, selected = true) {
        if (options.length > 0) {
            return (
                <Box>
                    <Typography variant="body1">
                        {selected ? 
                            `${text.youAlsoNeedToSelect}: `
                            :
                            `${text.theseOptionsWillBeRemoved}: `
                        }
                    </Typography>
                    <Grid container direction="row" justifyContent="center" alignItems="center">
                        {options.map(optionId => (
                            <OptionListItem key={optionId} optionId={optionId} highlight={selected ? 'add' : 'remove'}></OptionListItem>
                        ))}
                    </Grid>
                </Box>
            )
        }
        return ''
    }


    return (
        <Dialog
            open={isOpen}
            onClose={handleClose}
            scroll="paper"
            aria-labelledby="responsive-dialog-title"
            fullWidth
        >
            <DialogTitle id="responsive-dialog-title">
                {text.confirmationPrompt}
            </DialogTitle>

            <DialogContent dividers={true}>
                {renderDialogContent()}
            </DialogContent>

            <DialogActions>
                <Button autoFocus onClick={handleClose}>
                    {text.cancel}
                </Button>
                <Button autoFocus onClick={handleConfirm}>
                    {text.confirm}
                </Button>
            </DialogActions>
        </Dialog>
    )
}

const mapStateToProps = (state) => {
    const language = selectLanguage(state)
    return {
        content: confirmDialogGetBody(),
        message: selectConfirmDialogMessage(state),
        isOpen: selectIsConfirmDialogOpen(state),
        selectedOption: selectConfirmDialogData(state).selected,
        deselectedOption: selectConfirmDialogData(state).deselected,
        optionsToSelect: selectConfirmDialogData(state).optionsToSelect,
        optionsToRemove: selectConfirmDialogData(state).optionsToRemove,
        text: {
            cancel: translate('cancel', language),
            confirm: translate('confirm', language),
            confirmationPrompt: translate('confirmationPrompt', language),
            youWantToSelect: translate('youWantToSelect', language),
            youWantToRemove: translate('youWantToRemove', language),
            theseOptionsWillBeRemoved: translate('theseOptionsWillBeRemoved', language),
            youAlsoNeedToSelect: translate('youAlsoNeedToSelect', language)
        }
    }
}
const mapDispatchToProps = {
    cancel: confirmDialogCancel,
    confirm: confirmDialogConfirm,

    // cancel: useConfirmationDialog.cancel,
    // confirm: useConfirmationDialog.confirm,

    // openedDialog: openConfirmDialog,
    // closedDialog: closeConfirmDialog
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(ConfirmationOptionSelect)