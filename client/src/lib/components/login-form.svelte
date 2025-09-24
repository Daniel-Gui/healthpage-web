<script lang="ts">
	import HeartHandshake from "@lucide/svelte/icons/heart-handshake";
	import { Label } from "$lib/components/ui/label/index.js";
	import { Input } from "$lib/components/ui/input/index.js";
	import { Button } from "$lib/components/ui/button/index.js";
	import { cn, type WithElementRef } from "$lib/utils.js";
	import type { HTMLFormAttributes } from "svelte/elements";

	let {
		ref = $bindable(null),
		class: className,
		...restProps
	}: WithElementRef<HTMLFormAttributes> = $props();

	const id = $props.id();
</script>

<form class={cn("flex flex-col gap-6", className)} bind:this={ref} {...restProps}>
	<div class="flex flex-col items-center gap-2 text-center">
		<a href="##" class="flex items-center gap-2 font-medium mb-2">
				<div
					class="bg-primary text-primary-foreground flex size-6 items-center justify-center rounded-md"
				>
					<HeartHandshake class="size-4" />
				</div>
				HealthPage
			</a>
		<h1 class="text-2xl font-bold">Faça login na sua conta</h1>
		<p class="text-muted-foreground text-balance text-sm">
			Insira suas credenciais abaixo para fazer login
		</p>
	</div>
	<div class="grid gap-6">
		<div class="grid gap-3">
			<Label for="email-{id}">Email</Label>
			<Input id="email-{id}" type="email" placeholder="teste@exemplo.com" required />
		</div>
		<div class="grid gap-3">
			<div class="flex items-center">
				<Label for="password-{id}">Senha</Label>
				<a href="##" class="ml-auto text-sm underline-offset-4 hover:underline">
					Esqueceu sua senha?
				</a>
			</div>
			<Input id="password-{id}" type="password" required />
		</div>
		<Button type="submit" class="w-full">Entrar</Button>
		
		
	</div>
	<div class="text-center text-sm">
		Não tem uma conta?
		<a href="/register" class="underline underline-offset-4"> Cadastre-se </a>
	</div>
</form>
