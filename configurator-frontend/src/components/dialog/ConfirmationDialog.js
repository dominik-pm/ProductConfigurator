import React from 'react'
import Button from '@mui/material/Button'
import Dialog from '@mui/material/Dialog'
import DialogActions from '@mui/material/DialogActions'
import DialogContent from '@mui/material/DialogContent'
import DialogContentText from '@mui/material/DialogContentText'
import DialogTitle from '@mui/material/DialogTitle'
import { selectConfirmDialogContent, selectConfirmDialogMessage, selectIsConfirmDialogOpen } from '../../state/confirmationDialog/confirmationSelectors'
import { connect } from 'react-redux'
import { dialogCancel, dialogConfirm } from '../../state/confirmationDialog/confirmationSlice'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { Grid, Typography } from '@mui/material'
import OptionListItem from '../configuration/Configurator/Options/OptionListItem'
import { Box } from '@mui/system'

function ConfirmationOptionSelect({ isOpen, message, optionsToSelect, optionsToRemove, selectedOption, deselectedOption, cancel, confirm, language }) {
    
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
            <DialogContentText>
                {message}
            </DialogContentText>
        )
    }
    function renderDialogContentHeader() {
        return (
            <>
                <Typography variant="body1">
                    {selectedOption ? 
                        `${translate('youWantToSelect', language)}: `
                        :
                        `${translate('youWantToRemove', language)}: `
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
                            `${translate('youAlsoNeedToSelect', language)}: `
                            :
                            `${translate('theseOptionsWillBeRemoved', language)}: `
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
        <div>
            <Dialog
                open={isOpen}
                onClose={handleClose}
                scroll="paper"
                aria-labelledby="responsive-dialog-title"
            >
                <DialogTitle id="responsive-dialog-title">
                    {translate('confirmationPrompt', language)}
                </DialogTitle>

                <DialogContent dividers={true}>
                        {renderDialogContent()}
                </DialogContent>

                <DialogActions>
                    <Button autoFocus onClick={handleClose}>
                        {translate('cancel', language)}
                    </Button>
                    <Button autoFocus onClick={handleConfirm}>
                        {translate('confirm', language)}
                    </Button>
                </DialogActions>
            </Dialog>
        </div>
    )
}

const mapStateToProps = (state) => ({
    message: selectConfirmDialogMessage(state),
    isOpen: selectIsConfirmDialogOpen(state),
    language: selectLanguage(state),
    selectedOption: selectConfirmDialogContent(state).selected,
    deselectedOption: selectConfirmDialogContent(state).deselected,
    optionsToSelect: selectConfirmDialogContent(state).optionsToSelect,
    optionsToRemove: selectConfirmDialogContent(state).optionsToRemove
})
const mapDispatchToProps = {
    cancel: dialogCancel,
    confirm: dialogConfirm,

    // cancel: useConfirmationDialog.cancel,
    // confirm: useConfirmationDialog.confirm,

    // openedDialog: openConfirmDialog,
    // closedDialog: closeConfirmDialog
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(ConfirmationOptionSelect)