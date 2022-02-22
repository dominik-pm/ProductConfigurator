import { createSelector } from '@reduxjs/toolkit'

export const selectConfiguration = (state) =>           state.builder.configuration
export const selectBuilderStatus = (state) =>           state.builder.status
export const selectBuilderError = (state) =>            state.builder.error

export const selectBuilderSections = (state) =>         state.builder.configuration.optionSections
export const selectBuilderGroups = (state) =>           state.builder.configuration.optionGroups

const selectName = (state, name) =>                     name

export const getDoesSectionExist = createSelector([selectBuilderSections, selectName], (sections, sectionName) => {
    return sections.find(s => s.id === sectionName) ? true : false
})
export const getDoesGroupdExist = createSelector([selectBuilderGroups, selectName], (groups, groupName) => {
    return groups.find(g => g.id === groupName) ? true : false
})