
import { useState, useEffect } from 'react'
import { Card, message, Button } from 'antd'
import SearchFilters from '../components/SearchFilters'
import ResultsTable from '../components/ResultsTable'
import { search } from '../api/searchApi'
import { getVotes } from '../api/voteApi'
import { useAuth } from '../context/AuthContext'
import type { SearchQuery, SearchResult } from '../types'

export default function SearchPage() {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<SearchResult | null>(null);
  const [currentQuery, setCurrentQuery] = useState<SearchQuery>({
    sortBy: 'date',
    sortOrder: 'desc',
    page: 1,
    pageSize: 20,
  });
  const [userVotes, setUserVotes] = useState<Record<string, 'UpVote' | 'DownVote'>>({});

  const fetchVotes = async (userId: string) => {
    try {
      const res = await getVotes(undefined, userId);
      // Map: { [submissionId]: voteType }
      const map: Record<string, 'UpVote' | 'DownVote'> = {};
      res.votes.forEach((v: any) => {
        // Accept both 'UpVote'/'DownVote' and 'UPVOTE'/'DOWNVOTE'
        if (v.voteType.toLowerCase() === 'upvote') map[v.submissionId] = 'UpVote';
        else if (v.voteType.toLowerCase() === 'downvote') map[v.submissionId] = 'DownVote';
      });
      setUserVotes(map);
    } catch {
      setUserVotes({});
    }
  };

  const handleSearch = async (query: SearchQuery) => {
    setCurrentQuery(query);
    setLoading(true);
    try {
      const data = await search(query);
      setResult(data);
      if (user?.userId) {
        await fetchVotes(user.userId);
      } else {
        setUserVotes({});
      }
    } catch {
      void message.error('Search failed. Make sure the Search API is running.');
    } finally {
      setLoading(false);
    }
  };

  const handlePageChange = (page: number, pageSize: number) => {
    handleSearch({ ...currentQuery, page, pageSize });
  };

  // On mount, fetch initial data
  useEffect(() => {
    handleSearch(currentQuery);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Prepare filtered data for upvoted and downvoted tables
  const allResults = result?.results ?? [];
  const upvotedResults = allResults.filter((r) => userVotes[r.id] === 'UpVote');
  const downvotedResults = allResults.filter((r) => userVotes[r.id] === 'DownVote');

  const [activeTab, setActiveTab] = useState<'all' | 'up' | 'down'>('all');

  let tableTitle = 'All Salaries';
  let tableResult = result;
  const pageSize = currentQuery.pageSize || 20;
  if (activeTab === 'up') {
    tableTitle = 'Upvoted Salaries';
    tableResult = {
      ...result,
      results: upvotedResults,
      totalCount: upvotedResults.length,
      page: 1,
      totalPages: 1,
      pageSize,
    };
  } else if (activeTab === 'down') {
    tableTitle = 'Downvoted Salaries';
    tableResult = {
      ...result,
      results: downvotedResults,
      totalCount: downvotedResults.length,
      page: 1,
      totalPages: 1,
      pageSize,
    };
  }

  return (
    <>
      <Card style={{ marginBottom: 24 }}>
        <SearchFilters onSearch={handleSearch} loading={loading} />
      </Card>
      <Card>
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16 }}>
          <h3 style={{ margin: 0 }}>{tableTitle}</h3>
          <div>
            <Button
              type={activeTab === 'all' ? 'primary' : 'default'}
              onClick={() => setActiveTab('all')}
              style={{ marginRight: 8 }}
            >
              All
            </Button>
            <Button
              type={activeTab === 'up' ? 'primary' : 'default'}
              onClick={() => setActiveTab('up')}
              style={{ marginRight: 8 }}
            >
              Up Voted
            </Button>
            <Button
              type={activeTab === 'down' ? 'primary' : 'default'}
              onClick={() => setActiveTab('down')}
            >
              Down Voted
            </Button>
          </div>
        </div>
        <ResultsTable
          result={tableResult}
          loading={loading}
          onPageChange={activeTab === 'all' ? handlePageChange : () => {}}
          userVotes={userVotes}
        />
      </Card>
    </>
  );
}
