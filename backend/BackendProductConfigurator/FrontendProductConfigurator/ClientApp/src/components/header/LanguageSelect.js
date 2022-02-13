import { FormControl, InputLabel, MenuItem, Select } from '@mui/material'
import { Box } from '@mui/system'
import React from 'react'
import { connect } from 'react-redux'
import { languageNames } from '../../lang'
import { selectLanguage } from '../../state/language/languageSelectors'
import { setLanguage } from '../../state/language/languageSlice'

function LanguageSelect({ language, setLanguage }) {

    function handleChange(event) {
        setLanguage(event.target.value)
    }

    return (
        <Box sx={{ minWidth: 120 }}>
            <FormControl variant='standard' fullWidth>
                <InputLabel id="select-lang-label">Language</InputLabel>
                <Select
                    labelId='select-lang-label'
                    value={language}
                    autoWidth
                    label="Lang"
                    onChange={handleChange}
                    >
                    <MenuItem value={languageNames.EN}>English</MenuItem>
                    <MenuItem value={languageNames.DE}>Deutsch</MenuItem>
                    <MenuItem value={languageNames.FR}>Français</MenuItem>
                </Select>
            </FormControl>
        </Box>
    )
}

const mapStateToProps = (state) => ({
    language: selectLanguage(state)
})
const mapDispatchToProps = {
    setLanguage
}
export default connect(
    mapStateToProps,
    mapDispatchToProps
)(LanguageSelect)
