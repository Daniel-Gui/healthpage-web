<script lang="ts">
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import type { WithoutChildren } from "$lib/utils.js";
	import type { ComponentProps } from "svelte";
	import type { Icon } from "@tabler/icons-svelte";
	import { page } from "$app/state";

	let {
		items,
		...restProps
	}: { items: { title: string; url: string; icon: Icon }[] } & WithoutChildren<
		ComponentProps<typeof Sidebar.Group>
	> = $props();

	// Função para verificar se o item está ativo baseado na URL atual
	function isItemActive(itemUrl: string): boolean {
		return page.url.pathname === itemUrl;
	}
</script>

<Sidebar.Group {...restProps}>
	<Sidebar.GroupContent>
		<Sidebar.Menu>
			{#each items as item (item.title)}
			<Sidebar.MenuItem>
				<Sidebar.MenuButton isActive={isItemActive(item.url)} href={item.url}>
					<item.icon />
					<span>{item.title}</span>
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		{/each}
		</Sidebar.Menu>
	</Sidebar.GroupContent>
</Sidebar.Group>
