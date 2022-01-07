import React from 'react'
import { connect } from 'react-redux'
import { selectCurrentAlert, selectIsAlertOpen } from '../../state/alert/alertSelectors'
import { closeAlert } from '../../state/alert/alertSlice'
import Swal from 'sweetalert2'

function GenericAlert({ isOpen, alert, close }) {

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        },
        didClose: () => {
            close()
        }
    })

    const renderAlert = () => {
        if (!isOpen) {
            return 
        }
        if (alert) {
            Toast.fire({
                icon: alert.type,
                title: alert.message
            })
        }
    }

    return (
        <div>
            {renderAlert()}
        </div>
    )
}
const mapStateToProps = (state) => ({
    isOpen: selectIsAlertOpen(state),
    alert: selectCurrentAlert(state)
})
const mapDispatchToProps = {
    close: closeAlert
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(GenericAlert)
