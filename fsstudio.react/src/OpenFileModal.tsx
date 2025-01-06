import React, { useState } from 'react';
import { useExecutionSession } from './components/SessionContext';
import { useNavigate } from 'react-router-dom';

export function OpenFileModal() {
    const { setRootFolder } = useExecutionSession()!;
    const navigate = useNavigate();
    const [filePath, setFilePath] = useState('');

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (filePath.trim()) {
            await setRootFolder(filePath);
            navigate('/');
        }
    };

    const handleClose = () => {
        navigate('/');
    };

    return (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.5)' }}>
            <div style={{ position: 'absolute', top: '30%', left: '30%', background: '#fff', padding: '20px' }}>
                <h2>Select File</h2>
                <form onSubmit={handleSubmit}>
                    <input
                        type="text"
                        value={filePath}
                        onChange={(e) => setFilePath(e.target.value)}
                        placeholder="Enter file path"
                    />
                    <button type="submit">Open</button>
                    <button type="button" onClick={handleClose}>Cancel</button>
                </form>
            </div>
        </div>
    );
}