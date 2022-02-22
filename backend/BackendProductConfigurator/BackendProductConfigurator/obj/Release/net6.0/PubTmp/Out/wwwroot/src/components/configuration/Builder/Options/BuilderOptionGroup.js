import { Typography } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { selectLanguage } from '../../../../state/language/languageSelectors'
import { translate } from '../../../../lang'
import { connect } from 'react-redux'
import BuilderOption from './BuilderOption'

function OptionGroup({group, language}) {
    const { name, description, optionIds } = group


    return (
        <Box marginBottom={1} padding={1}>
            <Box display={'flex'} justifyContent={'space-between'} alignItems={'center'}>
                <Typography variant="h3">
                    {name}
                </Typography>
            </Box>
            <Typography variant="subtitle2">{description}</Typography>
            <Box>
                {/* TODO: add Option Button */}
            </Box>
            <Box>
                {optionIds.map(optionId => (
                    <BuilderOption key={optionId} optionId={optionId}></BuilderOption>
                ))}
            </Box>
        </Box>
    )
}

const mapStateToProps = (state, ownProps) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {

}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(OptionGroup)
