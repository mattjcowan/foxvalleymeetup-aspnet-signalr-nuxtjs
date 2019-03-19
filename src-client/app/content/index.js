import { version, name, description } from '~/package.json'

export const nav = [
  {
    title: 'Home',
    base: '',
    exact: true
  },
  {
    title: 'Swagger',
    base: 'swagger/',
    exact: true
  }
  // {
  //   title: 'Directives',
  //   base: 'directives/',
  //   pages: directives
  // },
  // {
  //   title: 'Reference',
  //   base: 'reference/',
  //   pages: reference
  // },
  // {
  //   title: 'Misc',
  //   base: 'misc/',
  //   pages: misc
  // }
]

export { version, name, description }
