import { ListItem } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { getOptionsInSection } from '../../../../state/configuration/configurationSelectors'
import ModelOptionText from './ModelOptionText'

function SectionOptionList({ allOptions, selectedOptions }) {

    return (
        <>
            {allOptions.filter(o => selectedOptions.includes(o)).map(optionId => (
                <ListItem sx={{paddingBottom: 0, paddingTop: 0}} key={optionId}>
                    <ModelOptionText optionId={optionId}></ModelOptionText>
                </ListItem>
            ))}
        </>
    )
}

const mapStateToProps = (state, ownProps) => ({
    allOptions: getOptionsInSection(state, ownProps.sectionId)
})
const mapDispatchToProps = {

}

export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(SectionOptionList)
