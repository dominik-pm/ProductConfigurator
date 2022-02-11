import { Edit } from '@mui/icons-material'
import { Button, IconButton, Tooltip } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { inputDialogOpen } from '../../../state/inputDialog/inputDialogSlice'

function EditButton({ title, propertyName = '', oldValue = '', textButton = false, valueChangedCallback, openInputDialog }) {

    function handleClick() {
        const data = {
            item: {name: propertyName || 'new value', value: oldValue }
        }
        openInputDialog(`${title}`, data, (data) => {
            valueChangedCallback(data.item.value)
        })
    }

    function renderTextButton() {
        return (
            <Button onClick={handleClick}>{`${title}`}</Button>
        )
    }

    function renderIconButton() {
        return (
            <Tooltip title={title}>
                <IconButton sx={{display: 'inline'}} onClick={handleClick}>
                    <Edit />
                </IconButton>
            </Tooltip>
        )
    }

    return (
        <>
            {textButton ? renderTextButton() : renderIconButton()}
        </>
    )
}

const mapStateToProps = (state) => ({

})
const mapDispatchToProps = {
    openInputDialog: inputDialogOpen
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(EditButton)