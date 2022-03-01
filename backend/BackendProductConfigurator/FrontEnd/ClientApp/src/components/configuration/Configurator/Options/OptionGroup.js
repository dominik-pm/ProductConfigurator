import { Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import Option from './Option'
import GroupInvalidError from './GroupInvalidError'
import { getIsGroupValid } from '../../../../state/configuration/configurationSelectors'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { translate } from '../../../../lang'
import { connect } from 'react-redux'

function OptionGroup({group, isValid, groupError, language}) {
    const { name, description, optionIds } = group

    function renderGroupError() {
        if (!isValid) {
            return (
                <GroupInvalidError errorMessage={translate(groupError, language)}></GroupInvalidError>
            )
        } else {
            return
        }
    }

    return (
        <Box marginBottom={1} padding={1}>
            <Box display={'flex'} justifyContent={'space-between'} alignItems={'center'}>
                <Typography variant="h3">
                    {name}
                </Typography>
                {renderGroupError()}
            </Box>
            <Typography variant="subtitle2">{description}</Typography>
            <Box className="OptionContainer" sx={{display: 'grid', gap: '25px', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))'}}>
                {
                    optionIds.map(optionId => (
                        <Option key={optionId} optionId={optionId}></Option>
                    ))
                }
            </Box>
        </Box>
    )
}

const mapStateToProps = (state, ownProps) => {
    const groupError = getIsGroupValid(state, ownProps.group.id)
    const isValid = groupError === null

    return {
        isValid,
        groupError,
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(OptionGroup)
