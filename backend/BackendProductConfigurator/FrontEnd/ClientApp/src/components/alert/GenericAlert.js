import React from 'react'
import { connect } from 'react-redux'
import { selectCurrentAlert, selectIsAlertOpen } from '../../state/alert/alertSelectors'
import { closeAlert } from '../../state/alert/alertSlice'
import Swal from 'sweetalert2'

function GenericAlert({ isOpen, alert, close }) {

    // Alert settings
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
        // dont fire an alert if the alert is not open, there is no alert data or the alert is already visible
        if (!isOpen)            return
        if (!alert)             return
        if (Toast.isVisible())  return

        Toast.fire({
            icon: alert.type,
            title: alert.message
        })
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
