import { Delete } from '@mui/icons-material'
import { Grid, IconButton, Tooltip, Typography } from '@mui/material'
import React from 'react'
import { connect } from 'react-redux'
import { translate } from '../../../../lang'
import { getBuilderOptionById } from '../../../../state/configurationBuilder/builderSelectors'
import { deleteOption } from '../../../../state/configurationBuilder/builderSlice'
import { selectLanguage } from '../../../../state/language/languageSelectors'

function BuilderOption({ optionId, group, option, language, remove }) {

    const { name, description } = option

    function handleDelete() {
        remove(group.id, optionId)
    }

    return (
        <Grid container>
            <Grid item>
                <Typography variant="body1">{name}</Typography>
                <Typography variant="body2">{description}</Typography>
            </Grid>
            <Grid item>
                <Tooltip title={`${translate('remove', language)} '${name}'`}>
                    <IconButton onClick={handleDelete}>
                        <Delete />
                    </IconButton>
                </Tooltip>
            </Grid>
        </Grid>
    )
}

const mapStateToProps = (state, ownProps) => ({
    option: getBuilderOptionById(state, ownProps.optionId),
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    remove: deleteOption
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(BuilderOption)