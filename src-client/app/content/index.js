import { version, name, description } from '~/package.json'

export const nav = [
  {
    iclass: 'fa fa-bookmark',
    title: 'Bookmarks',
    to: '/',
    exact: true
  },
  {
    iclass: 'fa fa-book',
    title: 'Swagger',
    href: '/swagger/'
  }
]

export { version, name, description }
