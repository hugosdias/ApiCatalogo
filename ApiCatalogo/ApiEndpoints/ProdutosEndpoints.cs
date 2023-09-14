﻿using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.ApiEndpoints;

public static class ProdutosEndpoints
{
    public static void MapProdutosEndpoints(this WebApplication app)
    {
        app.MapPost("/produtos", async (Produto produto, AppDbContext db) =>
        {
            db.Produtos!.Add(produto);
            await db.SaveChangesAsync();

            return Results.Created($"/produtos/{produto.ProdutoId}", produto);
        });

        app.MapGet("/produtos", async (AppDbContext db) => await db.Produtos!.ToListAsync()).WithTags("Produtos").RequireAuthorization();

        app.MapGet("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            return await db.Produtos!.FindAsync(id)
                is Produto produto ? Results.Ok(produto) : Results.NotFound("Produto não localizado");
        });

        app.MapPut("/produtos/{id:int}", async (int id, Produto produto, AppDbContext db) =>
        {
            if (produto.ProdutoId != id)
            {
                Results.BadRequest();
            }

            var produtoDB = await db.Produtos!.FindAsync(id);

            if (produtoDB is null) return Results.NotFound();

            produtoDB.Nome = produto.Nome;
            produtoDB.Descricao = produto.Descricao;
            produtoDB.Preco = produto.Preco;
            produtoDB.Imagem = produto.Imagem;
            produtoDB.DataCompra = produto.DataCompra;
            produtoDB.Estoque = produto.Estoque;
            produtoDB.CategoriaId = produto.CategoriaId;

            await db.SaveChangesAsync();

            return Results.Ok(produtoDB);
        });

        app.MapDelete("/produtos/{id:int}", async (int id, AppDbContext db) =>
        {
            var produto = await db.Produtos!.FindAsync(id);

            if (produto is null)
            {
                return Results.NotFound();
            }

            db.Produtos.Remove(produto);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

    }
}
