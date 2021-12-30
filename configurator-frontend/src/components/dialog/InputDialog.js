import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { translate } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'

function InputDialog({ isOpen, dialogTitle, dialogConfirm, cancel, confirm, language }) {

    
    function handleClose() {
        cancel()
    }

    function handleConfirm() {
        confirm()
    }

    function renderDialogContent() {
        return (
            <Typography variant="body1">Test</Typography>
        )
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
                    {translate(dialogTitle, language)}
                </DialogTitle>

                <DialogContent dividers={true}>
                        {renderDialogContent()}
                </DialogContent>

                <DialogActions>
                    <Button autoFocus onClick={handleClose}>
                        {translate('cancel', language)}
                    </Button>
                    <Button autoFocus onClick={handleConfirm}>
                        {translate(dialogConfirm, language)}
                    </Button>
                </DialogActions>
            </Dialog>
        </div>
    )
}

const mapStateToProps = (state) => ({
    isOpen: false,
    language: selectLanguage(state),
    dialogTitle: 'Login',
    dialogConfirm: 'Login'
})
const mapDispatchToProps = {
    cancel: '',
    confirm: '',

    // cancel: useConfirmationDialog.cancel,
    // confirm: useConfirmationDialog.confirm,

    // openedDialog: openConfirmDialog,
    // closedDialog: closeConfirmDialog
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(InputDialog)