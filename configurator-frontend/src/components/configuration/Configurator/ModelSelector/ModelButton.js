import { Box, ButtonBase, List, ListSubheader, Typography } from '@mui/material'
import React, { useState } from 'react'
import { connect } from 'react-redux'
import { extractModelDescriptionFromModel, extractModelNameFromModel, extractModelOptionsFromModel, selectOptionSections, selectSelectedOptions } from '../../../../state/configuration/configurationSelectors'
import { setModel } from '../../../../state/configuration/configurationSlice'
import SectionOptionList from './SectionOptionList'

function ModelButton({ model, isSelected = false, disabled = false, selectedOptions, selectModel, sections = [] }) {

    // for the custom model, dont display sections if it is not selected
    if (!model && !isSelected) sections = []

    const name = model ? extractModelNameFromModel(model) : 'Custom'
    const description = model ? extractModelDescriptionFromModel(model) : ''
    const options = model ? extractModelOptionsFromModel(model) : (isSelected ? selectedOptions : [])

    const [hover, setHover] = useState(false)

    const border = isSelected ? '2px solid grey' : hover ? '1px solid grey' : '1px dashed grey'


    function handleClick() {
        if (!disabled) {
            selectModel(name)
        }
    }

    function handleHover(isHovering) {
        setHover(isHovering)
    }


    return (
        <ButtonBase 
            sx={{width: '100%', height: '100%'}}
            onMouseLeave={() => handleHover(false)} 
            onMouseOver={() => handleHover(true)}
            onClick={handleClick} 
        >
            <Box padding={2} sx={{ width: '100%', border: border }}>
                
                <Box height="80px">
                    <Typography variant="h4">
                        {name}
                    </Typography>
                    <Typography variant="body1">
                        {description}
                    </Typography>
                </Box>

                <List
                    dense={true}
                    sx={{
                        position: 'relative',
                        overflow: 'auto',
                        maxHeight: 300,
                        '& ul': { padding: 0 },
                    }}
                    subheader={<li />}
                >
                    {sections.map(section => (
                        <li key={section.id}>
                            <ul>
                                <ListSubheader>{section.name}</ListSubheader>
                                <SectionOptionList sectionId={section.id} selectedOptions={options}></SectionOptionList>
                            </ul>
                        </li>
                    ))}
                </List>

            </Box>
        </ButtonBase>
    )
}

const mapStateToProps = (state) => ({
    selectedOptions: selectSelectedOptions(state),
    sections: selectOptionSections(state)
})
const mapDispatchToProps = {
    selectModel: setModel,
}
export default connect(
    mapStateToProps, 
    mapDispatchToProps
)(ModelButton)
