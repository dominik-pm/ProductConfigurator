import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from '@mui/material'
import React, { useEffect, useState } from 'react'
import { connect } from 'react-redux'
import { translate } from '../../lang'
import { selectInputDialogData, selectInputDialogHeaderMessage, selectIsInputDialogOpen } from '../../state/inputDialog/inputDialogSelectors'
import { inputDialogCancel, inputDialogConfirm, inputDialogSetData } from '../../state/inputDialog/inputDialogSlice'
import { selectLanguage } from '../../state/language/languageSelectors'

function InputDialog({ isOpen, dialogTitle, inputData, cancel, confirm, setInputData, language }) {
    
    const [localData, setLocalData] = useState({...inputData})

    useEffect(() => {
        setLocalData({...inputData})
    }, [inputData])

    function valuesChanged(key, value) {
        setLocalData(prevData => ({
            ...prevData,
            [key]: {
                ...prevData[key],
                value
            }
        }))
    }
    
    function handleClose() {
        cancel()
    }

    function handleConfirm() {
        setInputData(localData)
        confirm()
    }

    function renderDialogContent() {
        // if there is no data (or not yet), return no content
        if (Object.keys(localData).length === 0) return (<></>)

        return (
            <form>
                {Object.keys(inputData).map((key, index) => (
                    <TextField
                    key={index}
                    autoFocus
                    autoComplete={inputData[key].isPassword ? "current-password" : "text"}
                    margin="dense"
                    label={inputData[key].name}
                    type={inputData[key].isEmail ? 'email' : inputData[key].isPassword ? 'password' : 'text'}
                        fullWidth
                        variant="standard"
                        value={localData[key].value}
                        onChange={(event) => valuesChanged(key, event.target.value)}
                    />

                 ))}
            </form>
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
                        {translate('submit', language)}
                    </Button>
                </DialogActions>
            </Dialog>
        </div>
    )
}

const mapStateToProps = (state) => ({
    inputData: selectInputDialogData(state),
    isOpen: selectIsInputDialogOpen(state),
    dialogTitle: selectInputDialogHeaderMessage(state),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    cancel: inputDialogCancel,
    confirm: inputDialogConfirm,
    setInputData: inputDialogSetData
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(InputDialog)