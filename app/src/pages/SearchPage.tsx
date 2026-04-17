import { useState } from 'react'
import { Card, message } from 'antd'
import SearchFilters from '../components/SearchFilters'
import ResultsTable from '../components/ResultsTable'
import { search } from '../api/searchApi'
import type { SearchQuery, SearchResult } from '../types'

export default function SearchPage() {
  const [loading, setLoading] = useState(false)
  const [result, setResult] = useState<SearchResult | null>(null)
  const [currentQuery, setCurrentQuery] = useState<SearchQuery>({
    sortBy: 'date',
    sortOrder: 'desc',
    page: 1,
    pageSize: 20,
  })

  const handleSearch = async (query: SearchQuery) => {
    setCurrentQuery(query)
    setLoading(true)
    try {
      const data = await search(query)
      setResult(data)
    } catch {
      void message.error('Search failed. Make sure the Search API is running.')
    } finally {
      setLoading(false)
    }
  }

  const handlePageChange = (page: number, pageSize: number) => {
    handleSearch({ ...currentQuery, page, pageSize })
  }

  return (
    <>
      <Card style={{ marginBottom: 24 }}>
        <SearchFilters onSearch={handleSearch} loading={loading} />
      </Card>
      <Card>
        <ResultsTable result={result} loading={loading} onPageChange={handlePageChange} />
      </Card>
    </>
  )
}
