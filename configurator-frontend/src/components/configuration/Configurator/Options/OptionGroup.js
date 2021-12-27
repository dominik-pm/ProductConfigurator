import { Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import Option from './Option'
import GroupInvalidError from './GroupInvalidError'
import './OptionGroup.css'
import { getIsGroupValid } from '../../../../state/configuration/configurationSelectors'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { translate } from '../../../../lang'
import { connect } from 'react-redux'

function OptionGroup({group, isValid, invalidReason, language}) {
    const { name, description, optionIds } = group

    function renderGroupError() {
        if (!isValid) {
            return (
                <GroupInvalidError errorMessage={translate(invalidReason, language)}></GroupInvalidError>
            )
        } else {
            return
        }
    }

    return (
        <Box className="OptionGroup" marginBottom={1} padding={1}>
            <Box className="OptionGroupTitle" display={'flex'} justifyContent={'space-between'} alignItems={'center'}>
                <Typography variant="h2">
                    {name}
                </Typography>
                {renderGroupError()}
            </Box>
            <Typography variant="subtitle2">{description}</Typography>
            <Box className="OptionContainer">
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
    const groupValidResult = getIsGroupValid(state, ownProps.group.id)
    return {
        isValid: groupValidResult[0],
        invalidReason: groupValidResult[1],
        language: selectLanguage(state)
    }
}
const mapDispatchToProps = {}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(OptionGroup)
