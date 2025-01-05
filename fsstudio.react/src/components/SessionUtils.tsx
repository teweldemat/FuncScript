import { SERVER_URL } from '../backend';
import { NodeState, SessionState } from './SessionContext';

export function findNodeByPath(session: SessionState, nodePath: string): NodeState | null {
    if (!nodePath || nodePath.trim() === '') {
        return session.rootNode;
    }
    const parts = nodePath.split('.');
    let current: NodeState = session.rootNode;

    for (const part of parts) {
        if (!current.childNodes) return null;
        const next = current.childNodes.find((n) => n.name === part);
        if (!next) return null;
        current = next;
    }
    return current;
}

export function updateSessionNode(rootNode: NodeState, nodePath: string, newNode: NodeState): NodeState {
    if (!nodePath || nodePath.trim() === '') {
        return newNode;
    }

    const clonedRoot = { ...rootNode };
    const parts = nodePath.split('.');
    let current = clonedRoot;

    for (let i = 0; i < parts.length; i++) {
        const part = parts[i];
        if (!current.childNodes) current.childNodes = [];
        const index = current.childNodes.findIndex((n) => n.name === part);
        if (index < 0) {
            return clonedRoot;
        }
        if (i === parts.length - 1) {
            current.childNodes[index] = newNode;
        } else {
            const childNode = { ...current.childNodes[index] };
            if (!childNode.childNodes) childNode.childNodes = [];
            childNode.childNodes = [...childNode.childNodes];
            current.childNodes[index] = childNode;
            current = childNode;
        }
    }
    return clonedRoot;
}

export async function listDirectory(path: string) {
    const url = `${SERVER_URL}/api/FileSystem/ListSubFoldersAndFiles?path=${encodeURIComponent(path)}`;
    const res = await fetch(url);
    if (!res.ok) throw new Error(await res.text());
    return await res.json();
}

export async function fetchChildren(sessionId: string): Promise<NodeState[]> {
    const res = await fetch(`${SERVER_URL}/api/sessions/${sessionId}/node/children`);
    if (!res.ok) {
        throw new Error(await res.text());
    }
    const childNodesRaw: any[] = await res.json();
    return childNodesRaw.map((cn: any) => {
        return {
            name: cn.name,
            expressionType: cn.expressionType ?? null,
            childrenCount: cn.childrenCount ?? 0,
            expression: null,
            dataLoaded: false,
            evaluating: false,
            evaluationRes: null,
            evaluationError: null,
            cachedValue: null,
            isCached: false,
            childNodes: null,
        };
    });
}

export async function createFolder(path: string, name: string) {
    const res = await fetch(`${SERVER_URL}/api/FileSystem/CreateFolder`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ path, name }),
    });
    if (!res.ok) throw new Error(await res.text());
}

export async function createFile(path: string, name: string) {
    const res = await fetch(`${SERVER_URL}/api/FileSystem/CreateFile`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ path, name }),
    });
    if (!res.ok) throw new Error(await res.text());
}

export async function duplicateFile(path: string, name: string) {
    const res = await fetch(`${SERVER_URL}/api/FileSystem/DuplicateFile`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ path, name }),
    });
    if (!res.ok) throw new Error(await res.text());
}
